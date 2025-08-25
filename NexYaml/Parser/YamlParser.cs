using System.Buffers;
using NexYaml.Core;
using NexYaml.Parser.States;
using NexYaml.Serialization;
using Silk.NET.OpenXR;

namespace NexYaml.Parser;

public enum ParseEventType : byte
{
    /// Reserved for internal use
    Nothing,
    StreamStart,
    StreamEnd,
    DocumentStart,
    DocumentEnd,
    /// Refer to an anchor ID
    Alias,
    /// Value, style, anchor_id, tag
    Scalar,
    /// Anchor ID
    SequenceStart,
    SequenceEnd,
    /// Anchor ID
    MappingStart,
    MappingEnd,
}

internal enum ParseState
{
    StreamStart,
    ImplicitDocumentStart,
    DocumentStart,
    DocumentContent,
    DocumentEnd,
    BlockNode,
    // BlockNodeOrIndentlessSequence,
    // FlowNode,
    BlockSequenceFirstEntry,
    BlockSequenceEntry,
    IndentlessSequenceEntry,
    BlockMappingFirstKey,
    BlockMappingKey,
    BlockMappingValue,
    FlowSequenceFirstEntry,
    FlowSequenceEntry,
    FlowSequenceEntryMappingKey,
    FlowSequenceEntryMappingValue,
    FlowSequenceEntryMappingEnd,
    FlowMappingFirstKey,
    FlowMappingKey,
    FlowMappingValue,
    FlowMappingEmptyValue,
    End,
}

public partial class YamlParser(ReadOnlySequence<char> sequence, IYamlSerializerResolver resolver) : IDisposable
{
    public static YamlParser FromSequence(in ReadOnlySequence<char> sequence, IYamlSerializerResolver resolver)
    {
        return new YamlParser(sequence, resolver);
    }

    public ParseEventType CurrentEventType { get; private set; } = default;

    public Marker CurrentMark => tokenizer.CurrentMark;
    public bool HasMapping(out ReadOnlySpan<char> mappingKey)
    {
        if (HasKeyMapping)
        {
            ValidateScalar(out mappingKey);
            return true;
        }
        mappingKey = default;
        return false;
    }
    /// <summary>
    /// Indicates if the current <see cref="ParseEventType.MappingEnd"/> or <see cref="ParseEventType.StreamEnd"/> has not happened yet.
    /// </summary>
    public bool HasKeyMapping => CurrentEventType is not ParseEventType.StreamEnd and not ParseEventType.MappingEnd;
    /// <summary>
    /// Indicates if the current <see cref="ParseEventType.SequenceEnd"/> or <see cref="ParseEventType.StreamEnd"/> has not happened yet.
    /// </summary>
    public bool HasSequence => CurrentEventType is not ParseEventType.StreamEnd and not ParseEventType.SequenceEnd;

    /// <summary>
    /// Validates the scalar and returns it if succesful.
    /// Else it's an empty scalar and <see cref="YamlException"/> will be thrown
    /// </summary>
    /// <param name="key">The <see cref="Scalar"/> Key of the Mapping</param>
    /// <returns></returns>
    /// <exception cref="YamlException">Throws when there is no <see cref="ParseEventType.Scalar"/> or if <see cref="TryGetScalarAsSpan(out ReadOnlySpan{byte})"/> doesn't succeed</exception>
    private void ValidateScalar(out ReadOnlySpan<char> key)
    {
        if (CurrentEventType != ParseEventType.Scalar)
        {
            throw new YamlException(CurrentMark, "Custom type deserialization supports only string key");
        }

        if (!TryGetScalarAsSpan(out key))
        {
            throw new YamlException(CurrentMark, "Custom type deserialization supports only string key");
        }
    }

    private TokenType CurrentTokenType => tokenizer.CurrentTokenType;

    private Utf8YamlTokenizer tokenizer = new Utf8YamlTokenizer(sequence);
    private ParseState currentState = ParseState.StreamStart;
    internal Scalar? currentScalar = null;
    private Tag? currentTag = null;
    private Anchor? currentAnchor = null;
    private int lastAnchorId = -1;
    private readonly Dictionary<string, int> anchors = [];
    private ExpandBuffer<ParseState> stateStack = new ExpandBuffer<ParseState>(16);

    public void Dispose()
    {
        tokenizer.Dispose();
        stateStack.Dispose();
        GC.SuppressFinalize(this);
    }

    private bool Read()
    {
        if (currentScalar is not null)
        {
            // tokenizer.ReturnToPool(currentScalar);
            // currentScalar = null;
        }

        if (currentState == ParseState.End)
        {
            CurrentEventType = ParseEventType.StreamEnd;
            return false;
        }

        switch (currentState)
        {
            case ParseState.StreamStart:
                State state = new StreamStart(this);
                var result = state.Parse(tokenizer);
                CurrentEventType = result.CurrentEvent;
                currentState = result.State;
                currentScalar = result.Scalar;
                break;

            case ParseState.ImplicitDocumentStart:
                State state2 = new ParseDocumentStartImplicit(this);
                var result2 = state2.Parse(tokenizer);
                CurrentEventType = result2.CurrentEvent;
                currentState = result2.State;
                currentScalar = result2.Scalar;
                currentTag = null;
                currentAnchor = null;
                break;

            case ParseState.DocumentStart:
                State state3 = new ParseStateDocumentStartExplicit(this);
                var result3 = state3.Parse(tokenizer);
                CurrentEventType = result3.CurrentEvent;
                currentState = result3.State;
                currentScalar = result3.Scalar;
                currentTag = null;
                currentAnchor = null;
                break;
            case ParseState.DocumentContent:
                ParseDocumentContent();
                break;

            case ParseState.DocumentEnd:
                State state4 = new ParseDocumentEnd(this);
                var result4 = state4.Parse(tokenizer);
                CurrentEventType = result4.CurrentEvent;
                currentState = result4.State;
                currentScalar = result4.Scalar;
                currentTag = null;
                currentAnchor = null;
                break;

            case ParseState.BlockNode:
                ParseNode(true, false);
                break;

            case ParseState.BlockMappingFirstKey:
                ParseBlockMappingKey(true);
                break;

            case ParseState.BlockMappingKey:
                ParseBlockMappingKey(false);
                break;

            case ParseState.BlockMappingValue:
                ParseBlockMappingValue();
                break;

            case ParseState.BlockSequenceFirstEntry:
                ParseBlockSequenceEntry(true);
                break;

            case ParseState.BlockSequenceEntry:
                ParseBlockSequenceEntry(false);
                break;

            case ParseState.FlowSequenceFirstEntry:
                ParseFlowSequenceEntry(true);
                break;

            case ParseState.FlowSequenceEntry:
                ParseFlowSequenceEntry(false);
                break;

            case ParseState.FlowMappingFirstKey:
                ParseFlowMappingKey(true);
                break;

            case ParseState.FlowMappingKey:
                ParseFlowMappingKey(false);
                break;

            case ParseState.FlowMappingValue:
                ParseFlowMappingValue(false);
                break;

            case ParseState.IndentlessSequenceEntry:
                ParseIndentlessSequenceEntry();
                break;

            case ParseState.FlowSequenceEntryMappingKey:
                ParseFlowSequenceEntryMappingKey();
                break;

            case ParseState.FlowSequenceEntryMappingValue:
                ParseFlowSequenceEntryMappingValue();
                break;

            case ParseState.FlowSequenceEntryMappingEnd:
                State state5 = new ParseFlowSequenceEntryMappingEnd(this);
                var result5 = state5.Parse(tokenizer);
                CurrentEventType = result5.CurrentEvent;
                currentState = result5.State;
                break;
            case ParseState.FlowMappingEmptyValue:
                ParseFlowMappingValue(true);
                break;

            case ParseState.End:
            default:
                throw new ArgumentOutOfRangeException($"The {nameof(CurrentEventType)} is {CurrentEventType} and it's out of range");
        }
        return true;
    }

    public void ReadWithVerify(ParseEventType eventType)
    {
        if (CurrentEventType != eventType)
            throw new YamlException(CurrentMark, $"Did not find expected event : `{eventType}`");
        Read();
    }

    public void SkipAfter(ParseEventType eventType)
    {
        while (CurrentEventType != eventType)
        {
            if (!Read())
            {
                break;
            }
        }
        if (CurrentEventType == eventType)
        {
            Read();
        }
    }

    public void SkipCurrentNode()
    {
        switch (CurrentEventType)
        {
            case ParseEventType.Alias:
            case ParseEventType.Scalar:
                Read();
                break;

            case ParseEventType.SequenceStart:
                {
                    var depth = 1;
                    while (Read())
                    {
                        switch (CurrentEventType)
                        {
                            case ParseEventType.SequenceStart:
                                ++depth;
                                break;
                            case ParseEventType.SequenceEnd when --depth <= 0:
                                Read();
                                return;
                        }
                    }
                    break;
                }
            case ParseEventType.MappingStart:
                {
                    var depth = 1;
                    while (Read())
                    {
                        switch (CurrentEventType)
                        {
                            case ParseEventType.MappingStart:
                                ++depth;
                                break;
                            case ParseEventType.MappingEnd when --depth <= 0:
                                Read();
                                return;
                        }
                    }
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException($"The {nameof(CurrentEventType)} is {CurrentEventType} and it's out of range");
        }
    }

    private void ParseDocumentContent()
    {
        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.VersionDirective:
            case TokenType.TagDirective:
            case TokenType.DocumentStart:
            case TokenType.DocumentEnd:
            case TokenType.StreamEnd:
                currentState = stateStack.Pop();
                currentScalar = null;
                CurrentEventType = ParseEventType.Scalar;
                break;
            default:
                ParseNode(true, false);
                break;
        }
    }

    private NextState ParseNode(bool block, bool indentlessSequence)
    {
        currentAnchor = null;
        currentTag = null;
        Anchor? anchor = null;
        Tag? Tag = null;
        ParseEventType type = ParseEventType.Scalar;
        ParseState state = ParseState.StreamStart;
        Scalar? scalar = null;

        switch (CurrentTokenType)
        {
            case TokenType.Alias:
                var state1 = stateStack.Pop();

                var name = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();  // TODO: Avoid `ToString`
                tokenizer.Read();

                if (anchors.TryGetValue(name, out var aliasId))
                {
                    return new(ParseEventType.Alias, state1, null, null, new Anchor(name, aliasId));
                }
                throw new YamlException(CurrentMark, "While parsing node, found unknown anchor");

            case TokenType.Anchor:
                {
                    var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString(); // TODO: Avoid `ToString`
                    var anchorId = RegisterAnchor(anchorName);
                    currentAnchor = anchor = new Anchor(anchorName, anchorId);
                    tokenizer.Read();
                    if (CurrentTokenType == TokenType.Tag)
                    {
                        currentTag = Tag = tokenizer.TakeCurrentTokenContent<Tag>();
                        tokenizer.Read();
                    }
                    break;
                }
            case TokenType.Tag:
                {
                    currentTag = tokenizer.TakeCurrentTokenContent<Tag>();
                    tokenizer.Read();
                    if (CurrentTokenType == TokenType.Anchor)
                    {
                        var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
                        tokenizer.Read();
                        var anchorId = RegisterAnchor(anchorName);
                        currentAnchor = anchor = new Anchor(anchorName, anchorId);
                    }
                    break;
                }
        }

        switch (CurrentTokenType)
        {
            case TokenType.BlockEntryStart when indentlessSequence:
                currentState = state = ParseState.IndentlessSequenceEntry;
                CurrentEventType = type = ParseEventType.SequenceStart;
                break;

            case TokenType.PlainScalar:
            case TokenType.FoldedScalar:
            case TokenType.LiteralScalar:
            case TokenType.SingleQuotedScaler:
            case TokenType.DoubleQuotedScaler:
                currentState = state = stateStack.Pop();
                CurrentEventType = type = ParseEventType.Scalar;
                currentScalar = scalar = tokenizer.TakeCurrentTokenContent<Scalar>();
                tokenizer.Read();
                break;

            case TokenType.FlowSequenceStart:
                currentState = state = ParseState.FlowSequenceFirstEntry;
                CurrentEventType = type = ParseEventType.SequenceStart;
                break;

            case TokenType.FlowMappingStart:
                currentState = state = ParseState.FlowMappingFirstKey;
                CurrentEventType = type = ParseEventType.MappingStart;
                break;

            case TokenType.BlockSequenceStart when block:
                currentState = state = ParseState.BlockSequenceFirstEntry;
                CurrentEventType = type = ParseEventType.SequenceStart;
                break;

            case TokenType.BlockMappingStart when block:
                currentState = state = ParseState.BlockMappingFirstKey;
                CurrentEventType = type = ParseEventType.MappingStart;
                break;

            // ex 7.2, an empty scalar can follow a secondary tag
            case var _ when currentAnchor != null || currentTag != null:
                currentState = state = stateStack.Pop();
                currentScalar = scalar = null;
                CurrentEventType = type = ParseEventType.Scalar;
                break;

            // consider empty entry in sequence ("- ") as null
            case TokenType.BlockEntryStart when currentState == ParseState.IndentlessSequenceEntry:
                currentState = state =stateStack.Pop();
                currentScalar = scalar = null;
                CurrentEventType = type = ParseEventType.Scalar;
                break;

            default:
                {
                    throw new YamlTokenizerException(tokenizer.CurrentMark,
                        "while parsing a node, did not find expected node content");
                }
        }
        return new NextState(CurrentEventType, state, scalar, Tag , anchor);
    }

    private void ParseBlockMappingKey(bool first)
    {
        // skip BlockMappingStart
        if (first)
        {
            tokenizer.Read();
        }

        switch (CurrentTokenType)
        {
            case TokenType.KeyStart:
                tokenizer.Read();
                if (CurrentTokenType is
                    TokenType.KeyStart or
                    TokenType.ValueStart or
                    TokenType.BlockEnd)
                {
                    currentState = ParseState.BlockMappingValue;
                    currentScalar = null;
                    CurrentEventType = ParseEventType.Scalar;
                }
                else
                {
                    PushState(ParseState.BlockMappingValue);
                    ParseNode(true, true);
                }
                break;

            case TokenType.ValueStart:
                currentState = ParseState.BlockMappingValue;
                currentScalar = null;
                CurrentEventType = ParseEventType.Scalar;
                break;

            case TokenType.BlockEnd:
                currentState = stateStack.Pop();
                tokenizer.Read();
                CurrentEventType = ParseEventType.MappingEnd;
                break;

            default:
                throw new YamlException(CurrentMark,
                    "while parsing a block mapping, did not find expected key");
        }
    }

    private void ParseBlockMappingValue()
    {
        if (CurrentTokenType == TokenType.ValueStart)
        {
            tokenizer.Read();
            if (CurrentTokenType is
                TokenType.KeyStart or
                TokenType.ValueStart or
                TokenType.BlockEnd)
            {
                currentState = ParseState.BlockMappingKey;
                currentScalar = null;
                CurrentEventType = ParseEventType.Scalar;
            }
            else
            {
                PushState(ParseState.BlockMappingKey);
                ParseNode(true, true);
            }
        }
        else
        {
            currentState = ParseState.BlockMappingKey;
            currentScalar = null;
            CurrentEventType = ParseEventType.Scalar;
        }
    }

    private void ParseBlockSequenceEntry(bool first)
    {
        // BLOCK-SEQUENCE-START
        if (first)
        {
            tokenizer.Read();
        }

        switch (CurrentTokenType)
        {
            case TokenType.BlockEnd:
                currentState = stateStack.Pop();
                tokenizer.Read();
                CurrentEventType = ParseEventType.SequenceEnd;
                break;

            case TokenType.BlockEntryStart:
                tokenizer.Read();
                if (CurrentTokenType is TokenType.BlockEntryStart or TokenType.BlockEnd)
                {
                    currentState = ParseState.BlockSequenceEntry;
                    currentScalar = null;
                    CurrentEventType = ParseEventType.Scalar;
                    break;
                }

                PushState(ParseState.BlockSequenceEntry);
                ParseNode(true, false);
                break;

            default:
                throw new YamlException(CurrentMark,
                    "while parsing a block collection, did not find expected '-' indicator");
        }
    }

    private void ParseFlowSequenceEntry(bool first)
    {
        // skip FlowMappingStart
        if (first)
        {
            tokenizer.Read();
        }

        switch (CurrentTokenType)
        {
            case TokenType.FlowSequenceEnd:
                currentState = stateStack.Pop();
                tokenizer.Read();
                CurrentEventType = ParseEventType.SequenceEnd;
                return;

            case TokenType.FlowEntryStart when !first:
                tokenizer.Read();
                break;

            default:
                if (!first)
                {
                    throw new YamlException(CurrentMark,
                        "while parsing a flow sequence, expected ',' or ']'");
                }
                break;
        }

        switch (CurrentTokenType)
        {
            case TokenType.FlowSequenceEnd:
                currentState = stateStack.Pop();
                tokenizer.Read();
                CurrentEventType = ParseEventType.SequenceEnd;
                break;

            case TokenType.KeyStart:
                currentState = ParseState.FlowSequenceEntryMappingKey;
                tokenizer.Read();
                CurrentEventType = ParseEventType.MappingStart;
                break;

            default:
                PushState(ParseState.FlowSequenceEntry);
                ParseNode(false, false);
                break;
        }
    }

    private void ParseFlowMappingKey(bool first)
    {
        if (first)
        {
            tokenizer.Read();
        }

        if (CurrentTokenType == TokenType.FlowMappingEnd)
        {
            currentState = stateStack.Pop();
            tokenizer.Read();
            CurrentEventType = ParseEventType.MappingEnd;
            return;
        }

        if (!first)
        {
            if (CurrentTokenType == TokenType.FlowEntryStart)
            {
                tokenizer.Read();
            }
            else
            {
                throw new YamlException(CurrentMark,
                    "While parsing a flow mapping, did not find expected ',' or '}'");
            }
        }

        switch (CurrentTokenType)
        {
            case TokenType.KeyStart:
                tokenizer.Read();
                if (CurrentTokenType is
                    TokenType.ValueStart or
                    TokenType.FlowEntryStart or
                    TokenType.FlowMappingEnd)
                {
                    currentState = ParseState.FlowMappingValue;
                    currentScalar = null;
                    CurrentEventType = ParseEventType.Scalar;
                    break;
                }
                PushState(ParseState.FlowMappingValue);
                ParseNode(false, false);
                break;

            case TokenType.ValueStart:
                currentState = ParseState.FlowMappingValue;
                currentScalar = null;
                CurrentEventType = ParseEventType.Scalar;
                break;

            case TokenType.FlowMappingEnd:
                currentState = stateStack.Pop();
                tokenizer.Read();
                CurrentEventType = ParseEventType.MappingEnd;
                break;

            default:
                PushState(ParseState.FlowMappingEmptyValue);
                ParseNode(false, false);
                break;
        }
    }

    private void ParseFlowMappingValue(bool empty)
    {
        if (empty)
        {
            currentState = ParseState.FlowMappingKey;
            currentScalar = null;
            CurrentEventType = ParseEventType.Scalar;
            return;
        }

        if (CurrentTokenType == TokenType.ValueStart)
        {
            tokenizer.Read();
            if (CurrentTokenType is not TokenType.FlowEntryStart and
                not TokenType.FlowMappingEnd)
            {
                PushState(ParseState.FlowMappingKey);
                ParseNode(false, false);
                return;
            }
        }

        currentState = ParseState.FlowMappingKey;
        currentScalar = null;
        CurrentEventType = ParseEventType.Scalar;
    }

    private void ParseIndentlessSequenceEntry()
    {
        if (CurrentTokenType != TokenType.BlockEntryStart)
        {
            currentState = stateStack.Pop();
            CurrentEventType = ParseEventType.SequenceEnd;
            return;
        }

        tokenizer.Read();

        if (CurrentTokenType is
            TokenType.KeyStart or
            TokenType.ValueStart or
            TokenType.BlockEnd)
        {
            currentState = ParseState.IndentlessSequenceEntry;
            currentScalar = null;
            CurrentEventType = ParseEventType.Scalar;
        }
        else
        {
            PushState(ParseState.IndentlessSequenceEntry);
            ParseNode(true, false);
        }
    }

    private void ParseFlowSequenceEntryMappingKey()
    {
        if (CurrentTokenType is
            TokenType.ValueStart or
            TokenType.FlowEntryStart or
            TokenType.FlowSequenceEnd)
        {
            tokenizer.Read();
            currentState = ParseState.FlowSequenceEntryMappingValue;
            currentScalar = null;
            CurrentEventType = ParseEventType.Scalar;
        }
        else
        {
            PushState(ParseState.FlowSequenceEntryMappingValue);
            ParseNode(false, false);
        }
    }

    private void ParseFlowSequenceEntryMappingValue()
    {
        if (CurrentTokenType == TokenType.ValueStart)
        {
            tokenizer.Read();
            currentState = ParseState.FlowSequenceEntryMappingValue;
            if (CurrentTokenType is
                TokenType.FlowEntryStart or
                TokenType.FlowSequenceEnd)
            {
                currentState = ParseState.FlowSequenceEntryMappingEnd;
                currentScalar = null;
                CurrentEventType = ParseEventType.Scalar;
            }
            else
            {
                PushState(ParseState.FlowSequenceEntryMappingEnd);
                ParseNode(false, false);
            }
        }
        else
        {
            currentState = ParseState.FlowSequenceEntryMappingEnd;
            currentScalar = null;
            CurrentEventType = ParseEventType.Scalar;
        }
    }

    internal void PushState(ParseState state)
    {
        stateStack.Add(state);
    }


    private int RegisterAnchor(string anchorName)
    {
        var newId = ++lastAnchorId;
        anchors[anchorName] = newId;
        return newId;
    }
}
