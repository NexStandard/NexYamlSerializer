#nullable enable
using Irony.Parsing;
using NexVYaml.Serialization;
using NexYaml.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NexVYaml.Parser;


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

enum ParseState
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

public partial class YamlParser(ReadOnlySequence<byte> sequence, IYamlFormatterResolver resolver) : IDisposable
{

    public static YamlParser FromSequence(in ReadOnlySequence<byte> sequence, IYamlFormatterResolver resolver)
    {
        return new YamlParser(sequence, resolver);
    }

    public ParseEventType CurrentEventType { get; private set; } = default;

    public Marker CurrentMark
    {
        get => tokenizer.CurrentMark;
    }
    public bool HasMapping(out ReadOnlySpan<byte> mappingKey)
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
    void ValidateScalar(out ReadOnlySpan<byte> key)
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
    TokenType CurrentTokenType
    {
        get => tokenizer.CurrentTokenType;
    }

    Utf8YamlTokenizer tokenizer = new Utf8YamlTokenizer(sequence);
    ParseState currentState = ParseState.StreamStart;
    internal Scalar? currentScalar = null;
    Tag? currentTag = null;
    Anchor? currentAnchor = null;
    int lastAnchorId = -1;

    readonly Dictionary<string, int> anchors = [];
    ExpandBuffer<ParseState> stateStack = new ExpandBuffer<ParseState>(16);

    public void Dispose()
    {
        tokenizer.Dispose();
        stateStack.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool Read()
    {
        if (currentScalar is { } scalar)
        {
            tokenizer.ReturnToPool(scalar);
            currentScalar = null;
        }

        if (currentState == ParseState.End)
        {
            CurrentEventType = ParseEventType.StreamEnd;
            return false;
        }

        switch (currentState)
        {
            case ParseState.StreamStart:
                ParseStreamStart();
                break;

            case ParseState.ImplicitDocumentStart:
                ParseDocumentStart(true);
                break;

            case ParseState.DocumentStart:
                ParseDocumentStart(false);
                break;

            case ParseState.DocumentContent:
                ParseDocumentContent();
                break;

            case ParseState.DocumentEnd:
                ParseDocumentEnd();
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
                ParseFlowSequenceEntryMappingEnd();
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

    void ParseStreamStart()
    {
        if (CurrentTokenType == TokenType.None)
        {
            tokenizer.Read();
        }
        ThrowIfCurrentTokenUnless(TokenType.StreamStart);
        currentState = ParseState.ImplicitDocumentStart;
        tokenizer.Read();
        CurrentEventType = ParseEventType.StreamStart;
    }

    void ParseDocumentStart(bool implicitStarted)
    {
        if (!implicitStarted)
        {
            while (tokenizer.CurrentTokenType == TokenType.DocumentEnd)
            {
                tokenizer.Read();
            }
        }

        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.StreamEnd:
                currentState = ParseState.End;
                tokenizer.Read();
                CurrentEventType = ParseEventType.StreamEnd;
                break;

            case TokenType.VersionDirective:
            case TokenType.TagDirective:
            case TokenType.DocumentStart:
                ParseExplicitDocumentStart();
                break;

            default:
                if (implicitStarted)
                {
                    ProcessDirectives();
                    PushState(ParseState.DocumentEnd);
                    currentState = ParseState.BlockNode;
                    CurrentEventType = ParseEventType.DocumentStart;
                }
                else
                {
                    ParseExplicitDocumentStart();
                }
                break;
        }
    }

    void ParseExplicitDocumentStart()
    {
        ProcessDirectives();
        ThrowIfCurrentTokenUnless(TokenType.DocumentStart);
        PushState(ParseState.DocumentEnd);
        currentState = ParseState.DocumentContent;
        tokenizer.Read();
        CurrentEventType = ParseEventType.DocumentStart;
    }

    void ParseDocumentContent()
    {
        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.VersionDirective:
            case TokenType.TagDirective:
            case TokenType.DocumentStart:
            case TokenType.DocumentEnd:
            case TokenType.StreamEnd:
                PopState();
                EmptyScalar();
                break;
            default:
                ParseNode(true, false);
                break;
        }
    }

    void ParseDocumentEnd()
    {
        // var _implicit = true;
        if (CurrentTokenType == TokenType.DocumentEnd)
        {
            // _implicit = false;
            tokenizer.Read();
        }

        // TODO tag handling
        currentState = ParseState.DocumentStart;
        CurrentEventType = ParseEventType.DocumentEnd;
    }

    void ParseNode(bool block, bool indentlessSequence)
    {
        currentAnchor = null;
        currentTag = null;

        switch (CurrentTokenType)
        {
            case TokenType.Alias:
                PopState();

                var name = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();  // TODO: Avoid `ToString`
                tokenizer.Read();

                if (anchors.TryGetValue(name, out var aliasId))
                {
                    currentAnchor = new Anchor(name, aliasId);
                    CurrentEventType = ParseEventType.Alias;
                    return;
                }
                throw new YamlException(CurrentMark, "While parsing node, found unknown anchor");

            case TokenType.Anchor:
                {
                    var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString(); // TODO: Avoid `ToString`
                    var anchorId = RegisterAnchor(anchorName);
                    currentAnchor = new Anchor(anchorName, anchorId);
                    tokenizer.Read();
                    if (CurrentTokenType == TokenType.Tag)
                    {
                        currentTag = tokenizer.TakeCurrentTokenContent<Tag>();
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
                        var anchorId = RegisterAnchor(anchorName);
                        currentAnchor = new Anchor(anchorName, anchorId);
                        tokenizer.Read();
                    }
                    break;
                }
        }

        switch (CurrentTokenType)
        {
            case TokenType.BlockEntryStart when indentlessSequence:
                currentState = ParseState.IndentlessSequenceEntry;
                CurrentEventType = ParseEventType.SequenceStart;
                break;

            case TokenType.PlainScalar:
            case TokenType.FoldedScalar:
            case TokenType.LiteralScalar:
            case TokenType.SingleQuotedScaler:
            case TokenType.DoubleQuotedScaler:
                PopState();
                currentScalar = tokenizer.TakeCurrentTokenContent<Scalar>();
                tokenizer.Read();
                CurrentEventType = ParseEventType.Scalar;
                break;

            case TokenType.FlowSequenceStart:
                currentState = ParseState.FlowSequenceFirstEntry;
                CurrentEventType = ParseEventType.SequenceStart;
                break;

            case TokenType.FlowMappingStart:
                currentState = ParseState.FlowMappingFirstKey;
                CurrentEventType = ParseEventType.MappingStart;
                break;

            case TokenType.BlockSequenceStart when block:
                currentState = ParseState.BlockSequenceFirstEntry;
                CurrentEventType = ParseEventType.SequenceStart;
                break;

            case TokenType.BlockMappingStart when block:
                currentState = ParseState.BlockMappingFirstKey;
                CurrentEventType = ParseEventType.MappingStart;
                break;

            // ex 7.2, an empty scalar can follow a secondary tag
            case var _ when currentAnchor != null || currentTag != null:
                PopState();
                EmptyScalar();
                break;

            // consider empty entry in sequence ("- ") as null
            case TokenType.BlockEntryStart when currentState == ParseState.IndentlessSequenceEntry:
                PopState();
                EmptyScalar();
                break;

            default:
                {
                    throw new YamlTokenizerException(tokenizer.CurrentMark,
                        "while parsing a node, did not find expected node content");
                }
        }
    }

    void ParseBlockMappingKey(bool first)
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
                    EmptyScalar();
                }
                else
                {
                    PushState(ParseState.BlockMappingValue);
                    ParseNode(true, true);
                }
                break;

            case TokenType.ValueStart:
                currentState = ParseState.BlockMappingValue;
                EmptyScalar();
                break;

            case TokenType.BlockEnd:
                PopState();
                tokenizer.Read();
                CurrentEventType = ParseEventType.MappingEnd;
                break;

            default:
                throw new YamlException(CurrentMark,
                    "while parsing a block mapping, did not find expected key");
        }
    }

    void ParseBlockMappingValue()
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
                EmptyScalar();
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
            EmptyScalar();
        }
    }

    void ParseBlockSequenceEntry(bool first)
    {
        // BLOCK-SEQUENCE-START
        if (first)
        {
            tokenizer.Read();
        }

        switch (CurrentTokenType)
        {
            case TokenType.BlockEnd:
                PopState();
                tokenizer.Read();
                CurrentEventType = ParseEventType.SequenceEnd;
                break;

            case TokenType.BlockEntryStart:
                tokenizer.Read();
                if (CurrentTokenType is TokenType.BlockEntryStart or TokenType.BlockEnd)
                {
                    currentState = ParseState.BlockSequenceEntry;
                    EmptyScalar();
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

    void ParseFlowSequenceEntry(bool first)
    {
        // skip FlowMappingStart
        if (first)
        {
            tokenizer.Read();
        }

        switch (CurrentTokenType)
        {
            case TokenType.FlowSequenceEnd:
                PopState();
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
                PopState();
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

    void ParseFlowMappingKey(bool first)
    {
        if (first)
        {
            tokenizer.Read();
        }

        if (CurrentTokenType == TokenType.FlowMappingEnd)
        {
            PopState();
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
                    EmptyScalar();
                    break;
                }
                PushState(ParseState.FlowMappingValue);
                ParseNode(false, false);
                break;

            case TokenType.ValueStart:
                currentState = ParseState.FlowMappingValue;
                EmptyScalar();
                break;

            case TokenType.FlowMappingEnd:
                PopState();
                tokenizer.Read();
                CurrentEventType = ParseEventType.MappingEnd;
                break;

            default:
                PushState(ParseState.FlowMappingEmptyValue);
                ParseNode(false, false);
                break;
        }
    }

    void ParseFlowMappingValue(bool empty)
    {
        if (empty)
        {
            currentState = ParseState.FlowMappingKey;
            EmptyScalar();
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
        EmptyScalar();
    }

    void ParseIndentlessSequenceEntry()
    {
        if (CurrentTokenType != TokenType.BlockEntryStart)
        {
            PopState();
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
            EmptyScalar();
        }
        else
        {
            PushState(ParseState.IndentlessSequenceEntry);
            ParseNode(true, false);
        }
    }

    void ParseFlowSequenceEntryMappingKey()
    {
        if (CurrentTokenType is
            TokenType.ValueStart or
            TokenType.FlowEntryStart or
            TokenType.FlowSequenceEnd)
        {
            tokenizer.Read();
            currentState = ParseState.FlowSequenceEntryMappingValue;
            EmptyScalar();
        }
        else
        {
            PushState(ParseState.FlowSequenceEntryMappingValue);
            ParseNode(false, false);
        }
    }

    void ParseFlowSequenceEntryMappingValue()
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
                EmptyScalar();
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
            EmptyScalar();
        }
    }

    void ParseFlowSequenceEntryMappingEnd()
    {
        currentState = ParseState.FlowSequenceEntry;
        CurrentEventType = ParseEventType.MappingEnd;
    }

    void PopState()
    {
        currentState = stateStack.Pop();
    }

    void PushState(ParseState state)
    {
        stateStack.Add(state);
    }

    void EmptyScalar()
    {
        currentScalar = null;
        CurrentEventType = ParseEventType.Scalar;
    }

    void ProcessDirectives()
    {
        while (true)
        {
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.VersionDirective:
                    // TODO:
                    break;
                case TokenType.TagDirective:
                    // TODO:
                    break;
                default:
                    return;
            }
            tokenizer.Read();
        }
    }

    public int RegisterAnchor(string anchorName)
    {
        var newId = ++lastAnchorId;
        anchors[anchorName] = newId;
        return newId;
    }

    void ThrowIfCurrentTokenUnless(TokenType expectedTokenType)
    {
        if (CurrentTokenType != expectedTokenType)
        {
            throw new YamlException(tokenizer.CurrentMark,
                $"Did not find expected token of  `{expectedTokenType}`");
        }
    }
}

