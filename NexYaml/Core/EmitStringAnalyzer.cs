using System.Buffers;
using System.Text;
using Stride.Core;

namespace NexYaml.Core;

internal readonly struct EmitStringInfo(int lines, bool needsQuotes, bool isReservedWord)
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

internal static class EmitStringAnalyzer
{
    private static ReadOnlySpan<char> SpecialTokens => [':', '{', '[', ']', ',', '#', '`', '"', ' ', '\''];
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



        return new EmitStringInfo(lines, needsQuotes, isReservedWord);
    }

    public static StringBuilder BuildLiteralScalar(ReadOnlySpan<char> originalValue, int indentCharCount)
    {
        // Decide chomping: if the value ends with '\n' use '-', else '+'
        char chompHint = (originalValue.Length > 0 && originalValue[^1] == '\n') ? '-' : '+';

        var sb = new StringBuilder();
        sb.Append('|');
        sb.Append(chompHint);
        sb.Append('\n');
        AppendWhiteSpace(sb, indentCharCount);

        for (int i = 0; i < originalValue.Length; i++)
        {
            char ch = originalValue[i];
            sb.Append(ch);

            if (ch == '\n' && i < originalValue.Length - 1)
                AppendWhiteSpace(sb, indentCharCount);
        }

        return sb;
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

