using System.Runtime.CompilerServices;
using System.Text;
using Stride.Core;

namespace NexYaml.Core;

public readonly struct EmitStringInfo(int lines, bool needsQuotes, bool isReservedWord)
{
    public readonly int Lines = lines;
    public readonly bool NeedsQuotes = needsQuotes;
    public readonly bool IsReservedWord = isReservedWord;

    public ScalarStyle SuggestScalarStyle()
    {
        if (Lines <= 1)
            return NeedsQuotes ? ScalarStyle.DoubleQuoted : ScalarStyle.Plain;
        return ScalarStyle.Literal;
    }
}

public static class EmitStringAnalyzer
{

    static char[] whiteSpaces =
    [
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
    ];

    public static EmitStringInfo Analyze(string value)
    {
        var chars = value.AsSpan();
        if (chars.Length <= 0)
            return new EmitStringInfo(0, true, false);

        var isReservedWord = IsReservedWord(value);

        var first = chars[0];
        var last = chars[^1];

        var needsQuotes = isReservedWord ||
                          first == YamlCodes.Space ||
                          last == YamlCodes.Space ||
                          first is '&' or '*' or '?' or '|' or '-' or '<' or '>' or '=' or '!' or '%' or '@' or '.';

        var lines = 1;
        foreach (var ch in chars)
        {
            switch (ch)
            {
                case ':':
                case '{':
                case '[':
                case ']':
                case ',':
                case '#':
                case '`':
                case '"':
                case ' ':
                case '\'':
                    needsQuotes = true;
                    break;
                case '\n':
                    lines++;
                    break;
            }
        }

        if (last == '\n')
            lines--;
        return new EmitStringInfo(lines, needsQuotes, isReservedWord);
    }

    public static StringBuilder BuildLiteralScalar(ReadOnlySpan<char> originalValue, int indentCharCount)
    {
        var chompHint = '\0';
        if (originalValue.Length > 0 && originalValue[^1] == '\n')
        {
            if (originalValue[^2] == '\n' ||
                originalValue[^2] == '\r' && originalValue[^3] == '\n')
            {
                chompHint = '+';
            }
        }
        else
        {
            chompHint = '-';
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append('|');
        if (chompHint > 0)
            stringBuilder.Append(chompHint);
        stringBuilder.Append('\n');
        AppendWhiteSpace(stringBuilder, indentCharCount);

        for (var i = 0; i < originalValue.Length; i++)
        {
            var ch = originalValue[i];
            stringBuilder.Append(ch);
            if (ch == '\n' && i < originalValue.Length - 1)
                AppendWhiteSpace(stringBuilder, indentCharCount);
        }

        if (chompHint == '-')
            stringBuilder.Append('\n');
        return stringBuilder;
    }

    static bool IsReservedWord(string value)
    {
        switch (value.Length)
        {
            case 1:
                if (value == "~")
                    return true;
                break;
            case 4:
                if (value is "null" or "Null" or "NULL" or "true" or "True" or "TRUE")
                    return true;
                break;
            case 5:
                if (value is "false" or "False" or "FALSE")
                    return true;
                break;
        }
        return false;
    }

    static void AppendWhiteSpace(StringBuilder stringBuilder, int length)
    {
        if (length > whiteSpaces.Length)
            whiteSpaces = Enumerable.Repeat(' ', length * 2).ToArray();
        stringBuilder.Append(whiteSpaces.AsSpan(0, length));
    }
}

