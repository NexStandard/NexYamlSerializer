#nullable enable
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using VYaml.Internal;

namespace VYaml.Parser
{
    class YamlTokenizerException : Exception
    {
        public YamlTokenizerException(in Marker marker, string message)
            : base($"{message} at {marker}")
        {
        }
    }

    struct SimpleKeyState
    {
        public bool Possible;
        public bool Required;
        public int TokenNumber;
        public Marker Start;
    }

    public class Utf8YamlTokenizer
    {
        public TokenType CurrentTokenType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => currentToken.Type;
        }

        public Marker CurrentMark
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => mark;
        }

        ReadOnlySequence<byte> data;
        Marker mark;
        Token currentToken;
        long position;
        bool streamStartProduced;
        bool streamEndProduced;
        byte currentCode;
        int indent;
        bool simpleKeyAllowed;
        int adjacentValueAllowedAt;
        int flowLevel;
        int tokensParsed;
        bool tokenAvailable;

        InsertionQueue<Token> tokens;
        ScalarPool scalarPool;
        ExpandBuffer<SimpleKeyState> simpleKeyCandidates;
        ExpandBuffer<int> indents;
        ExpandBuffer<byte> lineBreaksBuffer;

        public Utf8YamlTokenizer(in ReadOnlySequence<byte> sequence)
        {
            data = sequence;
            var reader = new SequenceReader<byte>(data);
            
            mark = new Marker(0, 1, 0);
            tokens = new InsertionQueue<Token>(16);
            simpleKeyCandidates = new ExpandBuffer<SimpleKeyState>(16);
            indents = new ExpandBuffer<int>(16);
            lineBreaksBuffer = new ExpandBuffer<byte>(64);
            scalarPool = new ScalarPool(32);

            indent = -1;
            flowLevel = 0;
            adjacentValueAllowedAt = 0;
            tokensParsed = 0;
            simpleKeyAllowed = false;
            streamStartProduced = false;
            streamEndProduced = false;
            tokenAvailable = false;

            currentToken = default;

            reader.TryPeek(out currentCode);
            StorePosition(ref reader);

        }
        void StorePosition(ref SequenceReader<byte> reader)
        {
            position = reader.Position.GetInteger();
        }
        public void Dispose()
        {
            scalarPool.Dispose();
            simpleKeyCandidates.Dispose();
            indents.Dispose();
            lineBreaksBuffer.Dispose();
        }

        public bool Read()
        {
            SequenceReader<byte> reader = new SequenceReader<byte>(data);
            reader.Advance(position);
            if (streamEndProduced)
            {
                return false;
            }

            if (!tokenAvailable)
            {
                ConsumeMoreTokens(ref reader);
            }

            if (currentToken.Content is Scalar scalar)
            {
                ReturnToPool(scalar);
            }
            currentToken = tokens.Dequeue();
            tokenAvailable = false;
            tokensParsed += 1;

            if (currentToken.Type == TokenType.StreamEnd)
            {
                streamEndProduced = true;
            }
            StorePosition(ref reader);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReturnToPool(Scalar scalar)
        {
            scalarPool.Return(scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T TakeCurrentTokenContent<T>() where T : ITokenContent
        {
            var result = currentToken;
            currentToken = default;
            return (T)result.Content!;
        }

        void ConsumeMoreTokens(ref SequenceReader<byte> reader)
        {
            while (true)
            {
                var needMore = tokens.Count <= 0;
                if (!needMore)
                {
                    StaleSimpleKeyCandidates();
                    for (var i = 0; i < simpleKeyCandidates.Length; i++)
                    {
                        ref var simpleKeyState = ref simpleKeyCandidates[i];
                        if (simpleKeyState.Possible && simpleKeyState.TokenNumber == tokensParsed)
                        {
                            needMore = true;
                            break;
                        }
                    }
                }
                if (!needMore)
                {
                    break;
                }
                ConsumeNextToken(ref reader);
            }
            tokenAvailable = true;
        }

        void ConsumeNextToken(ref SequenceReader<byte> reader)
        {
            if (!streamStartProduced)
            {
                ConsumeStreamStart();
                return;
            }

            SkipToNextToken(ref reader);
            StaleSimpleKeyCandidates();
            UnrollIndent(mark.Col);

            if (reader.End)
            {
                ConsumeStreamEnd();
                return;
            }

            if (mark.Col == 0)
            {
                switch (currentCode)
                {
                    case (byte)'%':
                        ConsumeDirective(ref reader);
                        return;
                    case (byte)'-' when reader.IsNext(YamlCodes.StreamStart) && IsEmptyNext(YamlCodes.StreamStart.Length,ref reader):
                        ConsumeDocumentIndicator(TokenType.DocumentStart,ref reader);
                        return;
                    case (byte)'.' when reader.IsNext(YamlCodes.DocStart) && IsEmptyNext(YamlCodes.DocStart.Length, ref reader):
                        ConsumeDocumentIndicator(TokenType.DocumentEnd, ref reader);
                        return;
                }
            }

            switch (currentCode)
            {
                case YamlCodes.FlowSequenceStart:
                    ConsumeFlowCollectionStart(TokenType.FlowSequenceStart,ref reader);
                    break;
                case YamlCodes.FlowMapStart:
                    ConsumeFlowCollectionStart(TokenType.FlowMappingStart, ref reader);
                    break;
                case YamlCodes.FlowSequenceEnd:
                    ConsumeFlowCollectionEnd(TokenType.FlowSequenceEnd, ref reader);
                    break;
                case YamlCodes.FlowMapEnd:
                    ConsumeFlowCollectionEnd(TokenType.FlowMappingEnd, ref reader);
                    break;
                case YamlCodes.Comma:
                    ConsumeFlowEntryStart(ref reader);
                    break;
                case YamlCodes.BlockEntryIndent when !TryPeek(1, out var nextCode,ref reader) ||
                                                     YamlCodes.IsEmpty(nextCode):
                    ConsumeBlockEntry(ref reader);
                    break;
                case YamlCodes.ExplicitKeyIndent when !TryPeek(1, out var nextCode, ref reader) ||
                                                      YamlCodes.IsEmpty(nextCode):
                    ConsumeComplexKeyStart(ref reader);
                    break;
                case YamlCodes.MapValueIndent
                    when (TryPeek(1, out var nextCode, ref reader) && YamlCodes.IsEmpty(nextCode)) ||
                         (flowLevel > 0 && (YamlCodes.IsAnyFlowSymbol(nextCode) || mark.Position == adjacentValueAllowedAt)):
                    ConsumeValueStart(ref reader);
                    break;
                case YamlCodes.Alias:
                    ConsumeAnchor(true,ref reader);
                    break;
                case YamlCodes.Anchor:
                    ConsumeAnchor(false,ref reader);
                    break;
                case YamlCodes.Tag:
                    ConsumeTag(ref reader);
                    break;
                case YamlCodes.LiteralScalerHeader when flowLevel == 0:
                    ConsumeBlockScaler(true,ref reader);
                    break;
                case YamlCodes.FoldedScalerHeader when flowLevel == 0:
                    ConsumeBlockScaler(false,ref reader);
                    break;
                case YamlCodes.SingleQuote:
                    ConsumeFlowScaler(true,ref reader);
                    break;
                case YamlCodes.DoubleQuote:
                    ConsumeFlowScaler(false, ref reader);
                    break;
                // Plain Scaler
                case YamlCodes.BlockEntryIndent when !TryPeek(1, out var nextCode, ref reader) ||
                                                     YamlCodes.IsBlank(nextCode):
                    ConsumePlainScaler(ref reader);
                    break;
                case YamlCodes.MapValueIndent or YamlCodes.ExplicitKeyIndent
                    when flowLevel == 0 &&
                         (!TryPeek(1, out var nextCode, ref reader) || YamlCodes.IsBlank(nextCode)):
                    ConsumePlainScaler(ref reader);
                    break;
                case (byte)'%' or (byte)'@' or (byte)'`':
                    throw new YamlTokenizerException(in mark, $"Unexpected character: '{currentCode}'");
                default:
                    ConsumePlainScaler(ref reader);
                    break;
            }
        }

        void ConsumeStreamStart()
        {
            indent = -1;
            streamStartProduced = true;
            simpleKeyAllowed = true;
            tokens.Enqueue(new Token(TokenType.StreamStart));
            simpleKeyCandidates.Add(new SimpleKeyState());
        }

        void ConsumeStreamEnd()
        {
            // force new line
            if (mark.Col != 0)
            {
                mark.Col = 0;
                mark.Line += 1;
            }
            UnrollIndent(-1);
            RemoveSimpleKeyCandidate();
            simpleKeyAllowed = false;
            tokens.Enqueue(new Token(TokenType.StreamEnd));
        }

        void ConsumeDirective(ref SequenceReader<byte> reader)
        {
            UnrollIndent(-1);
            RemoveSimpleKeyCandidate();
            simpleKeyAllowed = false;

            Advance(1, ref reader);

            var name = scalarPool.Rent();
            try
            {
                ConsumeDirectiveName(name,ref reader);
                if (name.SequenceEqual(YamlCodes.YamlDirectiveName))
                {
                    ConsumeVersionDirectiveValue(ref reader);
                }
                else if (name.SequenceEqual(YamlCodes.TagDirectiveName))
                {
                    ConsumeTagDirectiveValue(ref reader);
                }
                else
                {
                    // Skip current line
                    while (!reader.End && !YamlCodes.IsLineBreak(currentCode))
                    {
                        Advance(1, ref reader);
                    }

                    // TODO: This should be error ?
                    tokens.Enqueue(new Token(TokenType.TagDirective));
                }
            }
            finally
            {
                scalarPool.Return(name);
            }

            while (YamlCodes.IsBlank(currentCode))
            {
                Advance(1, ref reader);
            }

            if (currentCode == YamlCodes.Comment)
            {
                while (!reader.End && !YamlCodes.IsLineBreak(currentCode))
                {
                    Advance(1, ref reader);
                }
            }

            if (!reader.End && !YamlCodes.IsLineBreak(currentCode))
            {
                throw new YamlTokenizerException(CurrentMark,
                    "While scanning a directive, did not find expected comment or line break");
            }

            // Eat a line break
            if (YamlCodes.IsLineBreak(currentCode))
            {
                ConsumeLineBreaks(ref reader);
            }
        }

        void ConsumeDirectiveName(Scalar result, ref SequenceReader<byte> reader)
        {
            while (YamlCodes.IsAlphaNumericDashOrUnderscore(currentCode))
            {
                result.Write(currentCode);
                Advance(1, ref reader);
            }

            if (result.Length <= 0)
            {
                throw new YamlTokenizerException(CurrentMark,
                    "While scanning a directive, could not find expected directive name");
            }

            if (!reader.End && !YamlCodes.IsBlank(currentCode))
            {
                throw new YamlTokenizerException(CurrentMark,
                    "While scanning a directive, found unexpected non-alphabetical character");
            }
        }

        void ConsumeVersionDirectiveValue(ref SequenceReader<byte> reader)
        {
            while (YamlCodes.IsBlank(currentCode))
            {
                Advance(1, ref reader);
            }

            var major = ConsumeVersionDirectiveNumber(ref reader);

            if (currentCode != '.')
            {
                throw new YamlTokenizerException(CurrentMark,
                    "while scanning a YAML directive, did not find expected digit or '.' character");
            }

            Advance(1, ref reader);
            var minor = ConsumeVersionDirectiveNumber(ref reader);
            tokens.Enqueue(new Token(TokenType.VersionDirective, new VersionDirective(major, minor)));
        }

        int ConsumeVersionDirectiveNumber(ref SequenceReader<byte> reader)
        {
            var value = 0;
            var length = 0;
            while (YamlCodes.IsNumber(currentCode))
            {
                if (length + 1 > 9)
                {
                    throw new YamlTokenizerException(CurrentMark,
                        "While scanning a YAML directive, found exteremely long version number");
                }

                length++;
                value = value * 10 + YamlCodes.AsHex(currentCode);
                Advance(1,ref reader);
            }

            if (length == 0)
            {
                throw new YamlTokenizerException(CurrentMark,
                    "While scanning a YAML directive, did not find expected version number");
            }
            return value;
        }

        void ConsumeTagDirectiveValue(ref SequenceReader<byte> reader)
        {
            var handle = scalarPool.Rent();
            var suffix = scalarPool.Rent();
            try
            {
                // Eat whitespaces.
                while (YamlCodes.IsBlank(currentCode))
                {
                    Advance(1, ref reader);
                }

                ConsumeTagHandle(true, handle,ref reader);

                // Eat whitespaces
                while (YamlCodes.IsBlank(currentCode))
                {
                    Advance(1, ref reader);
                }

                ConsumeTagUri(ref reader, null, suffix);

                if (YamlCodes.IsEmpty(currentCode) || reader.End)
                {
                    tokens.Enqueue(new Token(TokenType.TagDirective, new Tag(handle.ToString(), suffix.ToString())));
                }
                else
                {
                    throw new YamlTokenizerException(CurrentMark,
                        "While scanning TAG, did not find expected whitespace or line break");
                }
            }
            finally
            {
                scalarPool.Return(handle);
                scalarPool.Return(suffix);
            }
        }

        void ConsumeDocumentIndicator(TokenType tokenType,ref SequenceReader<byte> reader)
        {
            UnrollIndent(-1);
            RemoveSimpleKeyCandidate();
            simpleKeyAllowed = false;
            Advance(3, ref reader);
            tokens.Enqueue(new Token(tokenType));
        }

        void ConsumeFlowCollectionStart(TokenType tokenType,ref SequenceReader<byte> reader)
        {
            // The indicators '[' and '{' may start a simple key.
            SaveSimpleKeyCandidate();
            IncreaseFlowLevel();

            simpleKeyAllowed = true;

            Advance(1, ref reader);
            tokens.Enqueue(new Token(tokenType));
        }

        void ConsumeFlowCollectionEnd(TokenType tokenType,ref SequenceReader<byte> reader)
        {
            RemoveSimpleKeyCandidate();
            DecreaseFlowLevel();

            simpleKeyAllowed = false;

            Advance(1, ref reader);
            tokens.Enqueue(new Token(tokenType));
        }

        void ConsumeFlowEntryStart(ref SequenceReader<byte> reader)
        {
            RemoveSimpleKeyCandidate();
            simpleKeyAllowed = true;

            Advance(1, ref reader);
            tokens.Enqueue(new Token(TokenType.FlowEntryStart));
        }

        void ConsumeBlockEntry(ref SequenceReader<byte> reader)
        {
            if (flowLevel != 0)
            {
                throw new YamlTokenizerException(in mark, "'-' is only valid inside a block");
            }
            // Check if we are allowed to start a new entry.
            if (!simpleKeyAllowed)
            {
                throw new YamlTokenizerException(in mark, "Block sequence entries are not allowed in this context");
            }
            RollIndent(mark.Col, new Token(TokenType.BlockSequenceStart));
            RemoveSimpleKeyCandidate();
            simpleKeyAllowed = true;
            Advance(1, ref reader);
            tokens.Enqueue(new Token(TokenType.BlockEntryStart));
        }

        void ConsumeComplexKeyStart(ref SequenceReader<byte> reader)
        {
            if (flowLevel == 0)
            {
                // Check if we are allowed to start a new key (not necessarily simple).
                if (!simpleKeyAllowed)
                {
                    throw new YamlTokenizerException(in mark, "Mapping keys are not allowed in this context");
                }
                RollIndent(mark.Col, new Token(TokenType.BlockMappingStart));
            }
            RemoveSimpleKeyCandidate();

            simpleKeyAllowed = flowLevel == 0;
            Advance(1,ref reader);
            tokens.Enqueue(new Token(TokenType.KeyStart));
        }

        void ConsumeValueStart(ref SequenceReader<byte> reader)
        {
            ref var simpleKey = ref simpleKeyCandidates[^1];
            if (simpleKey.Possible)
            {
                // insert simple key
                var token = new Token(TokenType.KeyStart);
                tokens.Insert(simpleKey.TokenNumber - tokensParsed, token);

                // Add the BLOCK-MAPPING-START token if needed
                RollIndent(simpleKey.Start.Col, new Token(TokenType.BlockMappingStart), simpleKey.TokenNumber);
                ref var lastKey = ref simpleKeyCandidates[^1];
                lastKey.Possible = false;
                simpleKeyAllowed = false;
            }
            else
            {
                // The ':' indicator follows a complex key.
                if (flowLevel == 0)
                {
                    if (!simpleKeyAllowed)
                    {
                        throw new YamlTokenizerException(in mark, "Mapping values are not allowed in this context");
                    }
                    RollIndent(mark.Col, new Token(TokenType.BlockMappingStart));
                }
                simpleKeyAllowed = flowLevel == 0;
            }
            Advance(1, ref reader);
            tokens.Enqueue(new Token(TokenType.ValueStart));
        }

        void ConsumeAnchor(bool alias, ref SequenceReader<byte> reader)
        {
            SaveSimpleKeyCandidate();
            simpleKeyAllowed = false;

            var scalar = scalarPool.Rent();
            Advance(1, ref reader);

            while (YamlCodes.IsAlphaNumericDashOrUnderscore(currentCode))
            {
                scalar.Write(currentCode);
                Advance(1, ref reader);
            }

            if (scalar.Length <= 0)
            {
                throw new YamlTokenizerException(mark,
                    "while scanning an anchor or alias, did not find expected alphabetic or numeric character");
            }

            if (!YamlCodes.IsEmpty(currentCode) &&
                !reader.End &&
                currentCode != '?' &&
                currentCode != ':' &&
                currentCode != ',' &&
                currentCode != ']' &&
                currentCode != '}' &&
                currentCode != '%' &&
                currentCode != '@' &&
                currentCode != '`')
            {
                throw new YamlTokenizerException(in mark,
                    "while scanning an anchor or alias, did not find expected alphabetic or numeric character");
            }

            tokens.Enqueue(alias
                ? new Token(TokenType.Alias, scalar)
                : new Token(TokenType.Anchor, scalar));
        }

        void ConsumeTag(ref SequenceReader<byte> reader)
        {
            SaveSimpleKeyCandidate();
            simpleKeyAllowed = false;

            var handle = scalarPool.Rent();
            var suffix = scalarPool.Rent();

            try
            {
                // Check if the tag is in the canonical form (verbatim).
                if (TryPeek(1, out var nextCode, ref reader) && nextCode == '<')
                {
                    // Eat '!<'
                    Advance(2, ref reader);
                    ConsumeTagUri(ref reader, null, suffix);

                    if (currentCode != '>')
                    {
                        throw new YamlTokenizerException(mark, "While scanning a tag, did not find the expected '>'");
                    }

                    Advance(1, ref reader);
                }
                else
                {
                    // The tag has either the '!suffix' or the '!handle!suffix'
                    ConsumeTagHandle(false, handle,ref reader);

                    // Check if it is, indeed, handle.
                    var handleSpan = handle.AsSpan();
                    if (handleSpan.Length >= 2 && handleSpan[0] == '!' && handleSpan[^1] == '!')
                    {
                        ConsumeTagUri(ref reader, null, suffix);
                    }
                    else
                    {
                        ConsumeTagUri(ref reader, handle, suffix);
                        handle.Clear();
                        handle.Write((byte)'!');
                        // A special case: the '!' tag.  Set the handle to '' and the
                        // suffix to '!'.
                        if (suffix.Length <= 0)
                        {
                            handle.Clear();
                            suffix.Clear();
                            suffix.Write((byte)'!');
                        }
                    }
                }

                if (YamlCodes.IsEmpty(currentCode) || reader.End)
                {
                    // ex 7.2, an empty scalar can follow a secondary tag
                    tokens.Enqueue(new Token(TokenType.Tag, new Tag(handle.ToString(), suffix.ToString())));
                }
                else
                {
                    throw new YamlTokenizerException(mark,
                        "While scanning a tag, did not find expected whitespace or line break");
                }
            }
            finally
            {
                scalarPool.Return(handle);
                scalarPool.Return(suffix);
            }
        }

        void ConsumeTagHandle(bool directive, Scalar buf,ref SequenceReader<byte> reader)
        {
            if (currentCode != '!')
            {
                throw new YamlTokenizerException(mark,
                    "While scanning a tag, did not find expected '!'");
            }

            buf.Write(currentCode);
            Advance(1, ref reader);

            while (YamlCodes.IsAlphaNumericDashOrUnderscore(currentCode))
            {
                buf.Write(currentCode);
                Advance(1, ref reader);
            }

            // Check if the trailing character is '!' and copy it.
            if (currentCode == '!')
            {
                buf.Write(currentCode);
                Advance(1, ref reader);
            }
            else if (directive)
            {
                if (!buf.SequenceEqual(stackalloc byte[] { (byte)'!' }))
                {
                    // It's either the '!' tag or not really a tag handle.  If it's a %TAG
                    // directive, it's an error.  If it's a tag token, it must be a part of
                    // URI.
                    throw new YamlTokenizerException(mark, "While parsing a tag directive, did not find expected '!'");
                }
            }
        }

        void ConsumeTagUri(ref SequenceReader<byte> reader, Scalar? head, Scalar uri)
        {
            // Copy the head if needed.
            // Note that we don't copy the leading '!' character.
            var length = head?.Length ?? 0;
            if (length > 1)
            {
                 uri.Write(head!.AsSpan(1, length - 1));
            }

            // The set of characters that may appear in URI is as follows:
            while (currentCode is
                       (byte)';' or (byte)'/' or (byte)'?' or (byte)':' or (byte)':' or (byte)'@' or (byte)'&' or
                       (byte)'=' or (byte)'+' or (byte)'$' or (byte)',' or (byte)'.' or (byte)'!' or (byte)'!' or
                       (byte)'~' or (byte)'*' or (byte)'\'' or (byte)'(' or (byte)')' or (byte)'[' or (byte)']' or
                       (byte)'%' ||
                   YamlCodes.IsAlphaNumericDashOrUnderscore(currentCode))
            {
                if (currentCode == '%')
                {
                    uri.WriteUnicodeCodepoint(ConsumeUriEscapes(ref reader));
                }
                else
                {
                    uri.Write(currentCode);
                    Advance(1, ref reader);
                }

                length++;
            }
        }

        // TODO: Use Uri
        int ConsumeUriEscapes(ref SequenceReader<byte> reader)
        {
            var width = 0;
            var codepoint = 0;

            while (!reader.End)
            {
                TryPeek(1, out var hexcode0, ref reader);
                TryPeek(2, out var hexcode1, ref reader);
                if (currentCode != '%' || !YamlCodes.IsHex(hexcode0) || !YamlCodes.IsHex(hexcode1))
                {
                    throw new YamlTokenizerException(mark, "While parsing a tag, did not find URI escaped octet");
                }

                var octet = (YamlCodes.AsHex(hexcode0) << 4) + YamlCodes.AsHex(hexcode1);
                if (width == 0)
                {
                    width = octet switch {
                        _ when (octet & 0b1000_0000) == 0b0000_0000 => 1,
                        _ when (octet & 0b1110_0000) == 0b1100_0000 => 2,
                        _ when (octet & 0b1111_0000) == 0b1110_0000 => 3,
                        _ when (octet & 0b1111_1000) == 0b1111_0000 => 4,
                        _ => throw new YamlTokenizerException(mark,
                            "While parsing a tag, found an incorrect leading utf8 octet")
                    };
                    codepoint = octet;
                }
                else
                {
                    if ((octet & 0xc0) != 0x80)
                    {
                        throw new YamlTokenizerException(mark,
                            "While parsing a tag, found an incorrect trailing utf8 octet");
                    }
                    codepoint = (currentCode << 8) + octet;
                }

                Advance(3, ref reader);

                width -= 1;
                if (width == 0)
                {
                    break;
                }
            }

            return codepoint;
        }

        void ConsumeBlockScaler(bool literal, ref SequenceReader<byte> reader)
        {
            SaveSimpleKeyCandidate();
            simpleKeyAllowed = true;

            var chomping = 0;
            var increment = 0;
            var blockIndent = 0;

            var trailingBlank = false;
            var leadingBlank = false;
            var leadingBreak = LineBreakState.None;
            var scalar = scalarPool.Rent();

            lineBreaksBuffer.Clear();

            // skip '|' or '>'
            Advance(1, ref reader);

            if (currentCode is (byte)'+' or (byte)'-')
            {
                chomping = currentCode == (byte)'+' ? 1 : -1;
                Advance(1, ref reader);
                if (YamlCodes.IsNumber(currentCode))
                {
                    if (currentCode == (byte)'0')
                    {
                        throw new YamlTokenizerException(in mark,
                            "While scanning a block scalar, found an indentation indicator equal to 0");
                    }

                    increment = YamlCodes.AsHex(currentCode);
                    Advance(1, ref reader);
                }
            }
            else if (YamlCodes.IsNumber(currentCode))
            {
                if (currentCode == (byte)'0')
                {
                    throw new YamlTokenizerException(in mark,
                        "While scanning a block scalar, found an indentation indicator equal to 0");
                }
                increment = YamlCodes.AsHex(currentCode);
                Advance(1, ref reader);

                if (currentCode is (byte)'+' or (byte)'-')
                {
                    chomping = currentCode == (byte)'+' ? 1 : -1;
                    Advance(1, ref reader);
                }
            }

            // Eat whitespaces and comments to the end of the line.
            while (YamlCodes.IsBlank(currentCode))
            {
                Advance(1, ref reader);
            }

            if (currentCode == YamlCodes.Comment)
            {
                while (!reader.End && !YamlCodes.IsLineBreak(currentCode))
                {
                    Advance(1, ref reader);
                }
            }

            // Check if we are at the end of the line.
            if (!reader.End && !YamlCodes.IsLineBreak(currentCode))
            {
                throw new YamlTokenizerException(in mark,
                    "While scanning a block scalar, did not find expected commnet or line break");
            }

            if (YamlCodes.IsLineBreak(currentCode))
            {
                ConsumeLineBreaks(ref reader);
            }

            if (increment > 0)
            {
                blockIndent = indent >= 0 ? indent + increment : increment;
            }

            // Scan the leading line breaks and determine the indentation level if needed.
            ConsumeBlockScalarBreaks(ref blockIndent, ref lineBreaksBuffer, ref reader);

            while (mark.Col == blockIndent)
            {
                // We are at the beginning of a non-empty line.
                trailingBlank = YamlCodes.IsBlank(currentCode);
                if (!literal &&
                    leadingBreak != LineBreakState.None &&
                    !leadingBlank &&
                    !trailingBlank)
                {
                    if (lineBreaksBuffer.Length <= 0)
                    {
                        scalar.Write(YamlCodes.Space);
                    }
                }
                else
                {
                    scalar.Write(leadingBreak);
                }

                scalar.Write(lineBreaksBuffer.AsSpan());
                leadingBlank = YamlCodes.IsBlank(currentCode);
                leadingBreak = LineBreakState.None;
                lineBreaksBuffer.Clear();

                while (!reader.End && !YamlCodes.IsLineBreak(currentCode))
                {
                    scalar.Write(currentCode);
                    Advance(1, ref reader);
                }
                // break on EOF
                if (reader.End) break;

                leadingBreak = ConsumeLineBreaks(ref reader);
                // Eat the following indentation spaces and line breaks.
                ConsumeBlockScalarBreaks(ref blockIndent, ref lineBreaksBuffer, ref reader);
            }

            // Chomp the tail.
            if (chomping != -1)
            {
                scalar.Write(leadingBreak);
            }
            if (chomping == 1)
            {
                scalar.Write(lineBreaksBuffer.AsSpan());
            }

            var tokenType = literal ? TokenType.LiteralScalar : TokenType.FoldedScalar;
            tokens.Enqueue(new Token(tokenType, scalar));
        }

        void ConsumeBlockScalarBreaks(ref int blockIndent, ref ExpandBuffer<byte> blockLineBreaks,ref SequenceReader<byte> reader)
        {
            var maxIndent = 0;
            while (true)
            {
                while ((blockIndent == 0 || mark.Col < blockIndent) &&
                       currentCode == YamlCodes.Space)
                {
                    Advance(1, ref reader);
                }

                if (mark.Col > maxIndent)
                {
                    maxIndent = mark.Col;
                }

                // Check for a tab character messing the indentation.
                if ((blockIndent == 0 || mark.Col < blockIndent) && currentCode == YamlCodes.Tab)
                {
                    throw new YamlTokenizerException(in mark,
                        "while scanning a block scalar, found a tab character where an indentation space is expected");
                }

                if (!YamlCodes.IsLineBreak(currentCode))
                {
                    break;
                }

                switch (ConsumeLineBreaks(ref reader))
                {
                    case LineBreakState.Lf:
                        blockLineBreaks.Add(YamlCodes.Lf);
                        break;
                    case LineBreakState.CrLf:
                        blockLineBreaks.Add(YamlCodes.Cr);
                        blockLineBreaks.Add(YamlCodes.Lf);
                        break;
                    case LineBreakState.Cr:
                        blockLineBreaks.Add(YamlCodes.Cr);
                        break;
                }
            }

            if (blockIndent == 0)
            {
                blockIndent = maxIndent;
                if (blockIndent < indent + 1)
                {
                    blockIndent = indent + 1;
                }
                else if (blockIndent < 1)
                {
                    blockIndent = 1;
                }
            }
        }

        void ConsumeFlowScaler(bool singleQuote, ref SequenceReader<byte> reader)
        {
            SaveSimpleKeyCandidate();
            simpleKeyAllowed = false;

            var leadingBreak = default(LineBreakState);
            var trailingBreak = default(LineBreakState);
            var isLeadingBlanks = false;
            var scalar = scalarPool.Rent();

            Span<byte> whitespaceBuffer = stackalloc byte[32];
            var whitespaceLength = 0;

            // Eat the left quote
            Advance(1, ref reader);

            while (true)
            {
                if (mark.Col == 0 &&
                    (reader.IsNext(YamlCodes.StreamStart) ||
                     reader.IsNext(YamlCodes.DocStart)) &&
                    !TryPeek(3, out _, ref reader))
                {
                    throw new YamlTokenizerException(mark,
                        "while scanning a quoted scalar, found unexpected document indicator");
                }

                if (reader.End)
                {
                    throw new YamlTokenizerException(mark,
                        "while scanning a quoted scalar, found unexpected end of stream");
                }

                isLeadingBlanks = false;

                // Consume non-blank characters
                while (!reader.End && !YamlCodes.IsEmpty(currentCode))
                {
                    switch (currentCode)
                    {
                        // Check for an escaped single quote
                        case YamlCodes.SingleQuote when TryPeek(1, out var nextCode, ref reader) &&
                                                        nextCode == YamlCodes.SingleQuote && singleQuote:
                            scalar.Write((byte)'\'');
                            Advance(2, ref reader);
                            break;
                        // Check for the right quote.
                        case YamlCodes.SingleQuote when singleQuote:
                        case YamlCodes.DoubleQuote when !singleQuote:
                            goto LOOPEND;
                        // Check for an escaped line break.
                        case (byte)'\\' when !singleQuote &&
                                             TryPeek(1, out var nextCode, ref reader) &&
                                             YamlCodes.IsLineBreak(nextCode):
                            Advance(1, ref reader);
                            ConsumeLineBreaks(ref reader);
                            isLeadingBlanks = true;
                            break;
                        // Check for an escape sequence.
                        case (byte)'\\' when !singleQuote:
                            var codeLength = 0;
                            TryPeek(1, out var escaped, ref reader);
                            switch (escaped)
                            {
                                case (byte)'0':
                                    scalar.Write((byte)'\0');
                                    break;
                                case (byte)'a':
                                    scalar.Write((byte)'\a');
                                    break;
                                case (byte)'b':
                                    scalar.Write((byte)'\b');
                                    break;
                                case (byte)'t':
                                    scalar.Write((byte)'\t');
                                    break;
                                case (byte)'n':
                                    scalar.Write((byte)'\n');
                                    break;
                                case (byte)'v':
                                    scalar.Write((byte)'\v');
                                    break;
                                case (byte)'f':
                                    scalar.Write((byte)'\f');
                                    break;
                                case (byte)'r':
                                    scalar.Write((byte)'\r');
                                    break;
                                case (byte)'e':
                                    scalar.Write(0x1b);
                                    break;
                                case (byte)' ':
                                    scalar.Write((byte)' ');
                                    break;
                                case (byte)'"':
                                    scalar.Write((byte)'"');
                                    break;
                                case (byte)'\'':
                                    scalar.Write((byte)'\'');
                                    break;
                                case (byte)'\\':
                                    scalar.Write((byte)'\\');
                                    break;
                                // NEL (#x85)
                                case (byte)'N':
                                    scalar.WriteUnicodeCodepoint(0x85);
                                    break;
                                // #xA0
                                case (byte)'_':
                                    scalar.WriteUnicodeCodepoint(0xA0);
                                    break;
                                // LS (#x2028)
                                case (byte)'L':
                                    scalar.WriteUnicodeCodepoint(0x2028);
                                    break;
                                // PS (#x2029)
                                case (byte)'P':
                                    scalar.WriteUnicodeCodepoint(0x2029);
                                    break;
                                case (byte)'x':
                                    codeLength = 2;
                                    break;
                                case (byte)'u':
                                    codeLength = 4;
                                    break;
                                case (byte)'U':
                                    codeLength = 8;
                                    break;
                                default:
                                    throw new YamlTokenizerException(mark,
                                        "while parsing a quoted scalar, found unknown escape character");
                            }

                            Advance(2, ref reader);
                            // Consume an arbitrary escape code.
                            if (codeLength > 0)
                            {
                                var codepoint = 0;
                                for (var i = 0; i < codeLength; i++)
                                {
                                    if (TryPeek(i, out var hex, ref reader) && YamlCodes.IsHex(hex))
                                    {
                                        codepoint = (codepoint << 4) + YamlCodes.AsHex(hex);
                                    }
                                    else
                                    {
                                        throw new YamlTokenizerException(mark,
                                            "While parsing a quoted scalar, did not find expected hexadecimal number");
                                    }
                                }
                                scalar.WriteUnicodeCodepoint(codepoint);
                            }

                            Advance(codeLength, ref reader);
                            break;
                        default:
                            scalar.Write(currentCode);
                            Advance(1, ref reader);
                            break;
                    }
                }

                // Consume blank characters.
                while (YamlCodes.IsBlank(currentCode) || YamlCodes.IsLineBreak(currentCode))
                {
                    if (YamlCodes.IsBlank(currentCode))
                    {
                        // Consume a space or a tab character.
                        if (!isLeadingBlanks)
                        {
                            if (whitespaceBuffer.Length <= whitespaceLength)
                            {
                                whitespaceBuffer = new byte[whitespaceBuffer.Length * 2];
                            }
                            whitespaceBuffer[whitespaceLength++] = currentCode;
                        }
                        Advance(1, ref reader);
                    }
                    else
                    {
                        // Check if it is a first line break.
                        if (isLeadingBlanks)
                        {
                            trailingBreak = ConsumeLineBreaks(ref reader);
                        }
                        else
                        {
                            whitespaceLength = 0;
                            leadingBreak = ConsumeLineBreaks(ref reader);
                            isLeadingBlanks = true;
                        }
                    }
                }

                // Join the whitespaces or fold line breaks.
                if (isLeadingBlanks)
                {
                    if (leadingBreak == LineBreakState.None)
                    {
                        scalar.Write(trailingBreak);
                        trailingBreak = LineBreakState.None;
                    }
                    else
                    {
                        if (trailingBreak == LineBreakState.None)
                        {
                            scalar.Write(YamlCodes.Space);
                        }
                        else
                        {
                            scalar.Write(trailingBreak);
                            trailingBreak = LineBreakState.None;
                        }
                        leadingBreak = LineBreakState.None;
                    }
                }
                else
                {
                    scalar.Write(whitespaceBuffer[..whitespaceLength]);
                    whitespaceLength = 0;
                }
            }

            // Eat the right quote
            LOOPEND:
            Advance(1, ref reader);
            simpleKeyAllowed = isLeadingBlanks;

            // From spec: To ensure JSON compatibility, if a key inside a flow mapping is JSON-like,
            // YAML allows the following value to be specified adjacent to the “:”.
            adjacentValueAllowedAt = mark.Position;

            tokens.Enqueue(new Token(singleQuote
                ? TokenType.SingleQuotedScaler
                : TokenType.DoubleQuotedScaler,
                scalar));
        }

        void ConsumePlainScaler(ref SequenceReader<byte> reader)
        {
            SaveSimpleKeyCandidate();
            simpleKeyAllowed = false;

            var currentIndent = indent + 1;
            var leadingBreak = default(LineBreakState);
            var trailingBreak = default(LineBreakState);
            var isLeadingBlanks = false;
            var scalar = scalarPool.Rent();

            Span<byte> whitespaceBuffer = stackalloc byte[32];
            var whitespaceLength = 0;

            while (true)
            {
                // Check for a document indicator
                if (mark.Col == 0)
                {
                    if (currentCode == (byte)'-' && reader.IsNext(YamlCodes.StreamStart) && IsEmptyNext(YamlCodes.StreamStart.Length, ref reader))
                    {
                        break;
                    }
                    if (currentCode == (byte)'.' && reader.IsNext(YamlCodes.DocStart) && IsEmptyNext(YamlCodes.DocStart.Length, ref reader))
                    {
                        break;
                    }
                }
                if (currentCode == YamlCodes.Comment)
                {
                    break;
                }

                while (!reader.End && !YamlCodes.IsEmpty(currentCode))
                {
                    if (currentCode == YamlCodes.MapValueIndent)
                    {
                        var hasNext = TryPeek(1, out var nextCode, ref reader);
                        if (!hasNext ||
                            YamlCodes.IsEmpty(nextCode) ||
                            (flowLevel > 0 && YamlCodes.IsAnyFlowSymbol(nextCode)))
                        {
                            break;
                        }
                    }
                    else if (flowLevel > 0 && YamlCodes.IsAnyFlowSymbol(currentCode))
                    {
                        break;
                    }

                    if (isLeadingBlanks || whitespaceLength > 0)
                    {
                        if (isLeadingBlanks)
                        {
                            if (leadingBreak == LineBreakState.None)
                            {
                                scalar.Write(trailingBreak);
                                trailingBreak = LineBreakState.None;
                            }
                            else
                            {
                                if (trailingBreak == LineBreakState.None)
                                {
                                    scalar.Write(YamlCodes.Space);
                                }
                                else
                                {
                                    scalar.Write(trailingBreak);
                                    trailingBreak = LineBreakState.None;
                                }
                                leadingBreak = LineBreakState.None;
                            }
                            isLeadingBlanks = false;
                        }
                        else
                        {
                            scalar.Write(whitespaceBuffer[..whitespaceLength]);
                            whitespaceLength = 0;
                        }
                    }

                    scalar.Write(currentCode);
                    Advance(1, ref reader);
                }

                // is the end?
                if (!YamlCodes.IsEmpty(currentCode))
                {
                    break;
                }

                // whitespaces or line-breaks
                while (YamlCodes.IsEmpty(currentCode))
                {
                    // whitespaces
                    if (YamlCodes.IsBlank(currentCode))
                    {
                        if (isLeadingBlanks && mark.Col < currentIndent && currentCode == YamlCodes.Tab)
                        {
                            throw new YamlTokenizerException(mark, "While scanning a plain scaler, found a tab");
                        }
                        if (!isLeadingBlanks)
                        {
                            // If the buffer on the stack is insufficient, it is decompressed.
                            // This is probably a very rare case.
                            if (whitespaceLength >= whitespaceBuffer.Length)
                            {
                                whitespaceBuffer = new byte[whitespaceBuffer.Length * 2];
                            }
                            whitespaceBuffer[whitespaceLength++] = currentCode;
                        }
                        Advance(1, ref reader);
                    }
                    // line-break
                    else
                    {
                        // Check if it is a first line break
                        if (isLeadingBlanks)
                        {
                            trailingBreak = ConsumeLineBreaks(ref reader);
                        }
                        else
                        {
                            leadingBreak = ConsumeLineBreaks(ref reader);
                            isLeadingBlanks = true;
                            whitespaceLength = 0;
                        }
                    }
                }

                // check indentation level
                if (flowLevel == 0 && mark.Col < currentIndent)
                {
                    break;
                }
            }

            simpleKeyAllowed = isLeadingBlanks;
            tokens.Enqueue(new Token(TokenType.PlainScalar, scalar));
        }

        void SkipToNextToken(ref SequenceReader<byte> reader)
        {
            while (true)
            {
                switch (currentCode)
                {
                    case YamlCodes.Space:
                        Advance(1, ref reader);
                        break;
                    case YamlCodes.Tab when flowLevel > 0 || !simpleKeyAllowed:
                        Advance(1, ref reader);
                        break;
                    case YamlCodes.Lf:
                    case YamlCodes.Cr:
                        ConsumeLineBreaks(ref reader);
                        if (flowLevel == 0) simpleKeyAllowed = true;
                        break;
                    case YamlCodes.Comment:
                        while (!reader.End && !YamlCodes.IsLineBreak(currentCode))
                        {
                            Advance(1, ref reader);
                        }
                        break;
                    case 0xFE when reader.IsNext(YamlCodes.Bom):
                        Advance(YamlCodes.Bom.Length, ref reader);
                        break;
                    default:
                        return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Advance(int offset, ref SequenceReader<byte> reader)
        {
            for (var i = 0; i < offset; i++)
            {
                mark.Position += 1;
                if (currentCode == YamlCodes.Lf)
                {
                    mark.Line += 1;
                    mark.Col = 0;
                }
                else
                {
                    mark.Col += 1;
                }
                reader.Advance(1);
                reader.TryPeek(out currentCode);
            }
        }

        LineBreakState ConsumeLineBreaks(ref SequenceReader<byte> reader)
        {
            if (reader.End)
                return LineBreakState.None;

            switch (currentCode)
            {
                case YamlCodes.Cr:
                    if (TryPeek(1, out var secondCode,ref reader) && secondCode == YamlCodes.Lf)
                    {
                        Advance(2, ref reader);
                        return LineBreakState.CrLf;
                    }
                    Advance(1, ref reader);
                    return LineBreakState.Cr;
                case YamlCodes.Lf:
                    Advance(1, ref reader);
                    return LineBreakState.Lf;
            }
            return LineBreakState.None;
        }

        void StaleSimpleKeyCandidates()
        {
            for (var i = 0; i < simpleKeyCandidates.Length; i++)
            {
                ref var simpleKey = ref simpleKeyCandidates[i];
                if (simpleKey.Possible &&
                    (simpleKey.Start.Line < mark.Line || simpleKey.Start.Position + 1024 < mark.Position))
                {
                    if (simpleKey.Required)
                    {
                        throw new YamlTokenizerException(mark, "Simple key expect ':'");
                    }
                    simpleKey.Possible = false;
                }
            }
        }

        void SaveSimpleKeyCandidate()
        {
            if (!simpleKeyAllowed)
            {
                return;
            }

            ref var last = ref simpleKeyCandidates[^1];
            if (last is { Possible: true, Required: true })
            {
                throw new YamlTokenizerException(mark, "Simple key expected");
            }

            simpleKeyCandidates[^1] = new SimpleKeyState
            {
                Start = mark,
                Possible = true,
                Required = flowLevel > 0 && indent == mark.Col,
                TokenNumber = tokensParsed + tokens.Count
            };
        }

        void RemoveSimpleKeyCandidate()
        {
            ref var last = ref simpleKeyCandidates[^1];
            if (last is { Possible: true, Required: true })
            {
                throw new YamlTokenizerException(mark, "Simple key expected");
            }
            last.Possible = false;
        }

        void RollIndent(int colTo, in Token nextToken, int insertNumber = -1)
        {
            if (flowLevel > 0 || indent >= colTo)
            {
                return;
            }

            indents.Add(indent);
            indent = colTo;
            if (insertNumber >= 0)
            {
                tokens.Insert(insertNumber - tokensParsed, nextToken);
            }
            else
            {
                tokens.Enqueue(nextToken);
            }
        }

        void UnrollIndent(int col)
        {
            if (flowLevel > 0)
            {
                return;
            }
            while (indent > col)
            {
                tokens.Enqueue(new Token(TokenType.BlockEnd));
                indent = indents.Pop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IncreaseFlowLevel()
        {
            simpleKeyCandidates.Add(new SimpleKeyState());
            flowLevel++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DecreaseFlowLevel()
        {
            if (flowLevel <= 0) return;
            flowLevel--;
            simpleKeyCandidates.Pop();
        }

        bool IsEmptyNext(int offset, ref SequenceReader<byte> reader)
        {
            if (reader.End || reader.Remaining <= offset)
                return true;

            // If offset doesn't fall inside current segment move to next until we find correct one
            if (reader.CurrentSpanIndex + offset <= reader.CurrentSpan.Length - 1)
            {
                var nextCode = reader.CurrentSpan[reader.CurrentSpanIndex + offset];
                return YamlCodes.IsEmpty(nextCode);
            }

            var remainingOffset = offset;
            var nextPosition = reader.Position;
            ReadOnlyMemory<byte> currentMemory;

            while (reader.Sequence.TryGet(ref nextPosition, out currentMemory, advance: true))
            {
                // Skip empty segment
                if (currentMemory.Length > 0)
                {
                    if (remainingOffset >= currentMemory.Length)
                    {
                        // Subtract current non consumed data
                        remainingOffset -= currentMemory.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return YamlCodes.IsEmpty(currentMemory.Span[remainingOffset]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryPeek(long offset, out byte value, ref SequenceReader<byte> reader)
        {
            // If we've got data and offset is not out of bounds
            if (reader.End || reader.Remaining <= offset)
            {
                value = default;
                return false;
            }

            // If offset doesn't fall inside current segment move to next until we find correct one
            if (reader.CurrentSpanIndex + offset <= reader.CurrentSpan.Length - 1)
            {
                value = reader.CurrentSpan[reader.CurrentSpanIndex + (int)offset];
                return true;
            }

            var remainingOffset = offset;
            var nextPosition = reader.Position;
            ReadOnlyMemory<byte> currentMemory;

            while (reader.Sequence.TryGet(ref nextPosition, out currentMemory, advance: true))
            {
                // Skip empty segment
                if (currentMemory.Length > 0)
                {
                    if (remainingOffset >= currentMemory.Length)
                    {
                        // Subtract current non consumed data
                        remainingOffset -= currentMemory.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            value = currentMemory.Span[(int)remainingOffset];
            return true;
        }
    }
}

