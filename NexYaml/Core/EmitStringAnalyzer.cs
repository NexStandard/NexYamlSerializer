using Stride.Core;
using System.Buffers;
using System.Text;

namespace NexYaml.Core;

public readonly struct EmitStringInfo(int lines, bool needsQuotes, bool isReservedWord)
{
    public int Lines { get; } = lines;
    public bool NeedsQuotes { get; } = needsQuotes;
    public bool IsReservedWord { get; } = isReservedWord;

    public ScalarStyle SuggestScalarStyle()
    {
        if (Lines <= 1)
            return NeedsQuotes ? ScalarStyle.DoubleQuoted : ScalarStyle.Plain;
        return ScalarStyle.Literal;
    }
}

public static class EmitStringAnalyzer
{
    private static ReadOnlySpan<char> SpecialTokens => [':', '{', '[', ']', ',', '#', '`', '"', ' ','\''];
    private static char[] whiteSpaces =
    [
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
    ];

    public static EmitStringInfo Analyze(string value)
    {
        if (value.Length <= 0)
            return new EmitStringInfo(0, true, false);

        var isReservedWord = IsReservedWord(value);

        var first = value[0];
        var last = value[^1];
        var span = value.AsSpan();

        var needsQuotes = isReservedWord ||
                          first == YamlCodes.Space ||
                          last == YamlCodes.Space ||
                          first is '&' or '*' or '?' or '|' or '-' or '<' or '>' or '=' or '!' or '%' or '@' or '.' ||
                          span.ContainsAny(SpecialTokens);

        var lines = span.Count('\n');

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
                (originalValue[^2] == '\r' && originalValue[^3] == '\n'))
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

    private static bool IsReservedWord(string value)
    {
        return value is "~" or "null" or "Null" or "NULL" or "true" or "True" or "TRUE" or "false" or "False" or "FALSE";
    }

    private static void AppendWhiteSpace(StringBuilder stringBuilder, int length)
    {
        if (length > whiteSpaces.Length)
            whiteSpaces = Enumerable.Repeat(' ', length * 2).ToArray();
        stringBuilder.Append(whiteSpaces.AsSpan(0, length));
    }
}

