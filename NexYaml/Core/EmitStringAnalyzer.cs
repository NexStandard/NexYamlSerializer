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

    /// <summary>
    /// Builds a YAML literal block scalar representation from the given value.
    /// </summary>
    /// <param name="originalValue">
    /// The raw scalar text to encode. Line breaks are preserved and indented
    /// according to <paramref name="indentCharCount"/>.
    /// </param>
    /// <param name="indentCharCount">
    /// The number of spaces to insert at the start of each content line
    /// inside the literal block.
    /// </param>
    /// <returns>
    /// A <see cref="StringBuilder"/> containing the YAML literal block scalar,
    /// including the leading <c>|</c> and an appropriate chomping indicator.
    /// </returns>
    /// <remarks>
    /// Chomping is used to let the parser know if the trailing linebreak was written by the <see cref="NexYaml.Serialization.Node"/> OR if it's content of the literal scalar
    /// The method automatically selects the chomping indicator:
    /// <list type="bullet">
    ///   <item><description>
    /// If <paramref name="originalValue"/> ends with a newline (<c>'\n'</c>),
    /// the <c>|-</c> form is used so that the parser trims does <c>NOT</c> trim the trailing newline.
    /// </description></item>
    ///   <item><description>
    /// Otherwise, the <c>|+</c> form is used so that the parser trims the excessive \n
    /// </description></item>
    /// </list>
    /// Each line of <paramref name="originalValue"/> is indented by
    /// <paramref name="indentCharCount"/> spaces after the block header.
    /// </remarks>
    public static Span<char> BuildLiteralScalar(ReadOnlySpan<char> originalValue, int indentCharCount)
    {
        char chompHint = (originalValue.Length > 0 && originalValue[^1] == '\n') ? '-' : '+';

        // Precompute length
        int extraIndents = 0;
        for (int i = 0; i < originalValue.Length - 1; i++)
            if (originalValue[i] == '\n')
                extraIndents++;

        int totalLength = 1 + 1 + 1 + indentCharCount + originalValue.Length + extraIndents * indentCharCount;

        Span<char> span = new char[totalLength];

        int pos = 0;

        span[pos++] = '|';
        span[pos++] = chompHint;
        span[pos++] = '\n';

        for (int j = 0; j < indentCharCount; j++)
            span[pos++] = ' ';

        for (int i = 0; i < originalValue.Length; i++)
        {
            char ch = originalValue[i];
            span[pos++] = ch;

            if (ch == '\n' && i < originalValue.Length - 1)
            {
                for (int j = 0; j < indentCharCount; j++)
                    span[pos++] = ' ';
            }
        }

        return span;
    }

    private static bool IsReservedWord(string value)
    {
        return value is "~" or "null" or "Null" or "NULL" or "true" or "True" or "TRUE" or "false" or "False" or "FALSE";
    }

}

