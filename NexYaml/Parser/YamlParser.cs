using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using NexYaml.Core;
using NexYaml.Parser.States;
using NexYaml.Serialization;

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
    End
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

        //if (currentState == ParseState.End)
        //{
        //    CurrentEventType = ParseEventType.StreamEnd;
        //    return false;
        //}
        State state = currentState switch
        {
            ParseState.StreamStart => new StreamStart(this),
            ParseState.ImplicitDocumentStart => new ParseDocumentStartImplicit(this),
            ParseState.DocumentStart => new ParseStateDocumentStartExplicit(this),
            ParseState.DocumentContent => new FlowMapValue(this),
            ParseState.DocumentEnd => new ParseDocumentEnd(this),
            ParseState.BlockNode => new States.BlockNode(this, true, false),
            ParseState.BlockSequenceFirstEntry => new BlockSequenceFirstEntry(this),
            ParseState.BlockSequenceEntry => new BlockSequenceEntry(this),
            ParseState.IndentlessSequenceEntry => new IndentlessSequenceEntry(this),
            ParseState.BlockMappingFirstKey => new BlockMapFirstKey(this),
            ParseState.BlockMappingKey => new BlockMapKey(this),
            ParseState.BlockMappingValue => new BlockMapValue(this),
            ParseState.FlowSequenceFirstEntry => new FlowSequenceFirstEntry(this),
            ParseState.FlowSequenceEntry => new FlowSequenceEntry(this),
            ParseState.FlowSequenceEntryMappingKey => new FlowSequenceEntryMappingKey(this),
            ParseState.FlowSequenceEntryMappingValue => new FlowSequenceEntryMappingValue(this),
            ParseState.FlowSequenceEntryMappingEnd => new ParseFlowSequenceEntryMappingEnd(this),
            ParseState.FlowMappingFirstKey => new FlowMapFirstKey(this),
            ParseState.FlowMappingKey => new FlowMapKey(this),
            ParseState.FlowMappingValue => new FlowMapValue(this),
            ParseState.End => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException($"The {nameof(CurrentEventType)} is {CurrentEventType} and it's out of range"),

        };
        var result = state.Parse(tokenizer);
        CurrentEventType = result.CurrentEvent;
        currentState = result.State;
        currentTag = result.Tag;
        currentScalar = result.Scalar;
        currentAnchor = result.Anchor;
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


    internal NextState ParseNode(bool block, bool indentlessSequence)
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
                    currentTag = Tag = tokenizer.TakeCurrentTokenContent<Tag>();
                    tokenizer.Read();
                    if (CurrentTokenType == TokenType.Anchor)
                    {
                        tokenizer.Read();
                        var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
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

    internal ParseState Pop()
    {
        return stateStack.Pop();
    }


    internal bool TryResolveAnchor(string name,[MaybeNullWhen(false)] out Anchor? anchor)
    {
        if (anchors.TryGetValue(name, out var aliasId))
        {
            anchor = new Anchor(name, aliasId);
            return true;
        }
        anchor = default;
        return false;
    }

    internal int PushAnchor(string name)
    {
        return RegisterAnchor(name);
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
