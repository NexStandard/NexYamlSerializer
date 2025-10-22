namespace NexYaml.Core;

public static class YamlCodes
{
    public static readonly char[] YamlDirectiveName = ['Y', 'A', 'M', 'L'];
    public static readonly char[] TagDirectiveName = ['T', 'A', 'G'];

    public static readonly byte[] Bom = [0xFE, 0xFE];
    public static readonly char[] StreamStart = ['-', '-', '-'];
    public static readonly char[] DocStart = ['.', '.', '.'];
    public static readonly char[] CrLf = [Cr, Lf];

    public static readonly byte[] Null0 = [(byte)'!', (byte)'!', (byte)'n', (byte)'u', (byte)'l', (byte)'l'];
    public static readonly string Null = "!!null";

    public const char Space = ' ';
    public const char Tab = '\t';
    public const char Lf = '\n';
    public const char NewLine = '\n';
    public const char Cr = '\r';
    public const char Comment = '#';
    public const char DirectiveLine = '%';
    public const char Alias = '*';
    public const char Anchor = '&';
    public const char Tag = '!';
    public const char SingleQuote = '\'';
    public const char DoubleQuote = '"';
    public const char LiteralScalerHeader = '|';
    public const char FoldedScalerHeader = '>';
    public const char Comma = ',';
    public const char BlockEntryIndent = '-';
    public const char ExplicitKeyIndent = '?';
    public const char MapValueIndent = ':';
    public const char FlowMapStart = '{';
    public const char FlowMapEnd = '}';
    public const char FlowSequenceStart = '[';
    public const char FlowSequenceEnd = ']';

    public static bool IsAlphaNumericDashOrUnderscore(char code)
    {
        return code is
        (>= '0' and <= '9') or
        (>= 'A' and <= 'Z') or
        (>= 'a' and <= 'z') or
        '_' or
        '-';
    }

    public static bool IsNumber(char code)
    {
        return code is >= '0' and <= '9';
    }

    public static bool IsEmpty(char code)
    {
        return code is Space or Tab or Lf or Cr;
    }

    public static bool IsLineBreak(char code)
    {
        return code is Lf or Cr;
    }

    public static bool IsBlank(char code)
    {
        return code is Space or Tab;
    }

    public static bool IsNumberRepresentation(byte code)
    {
        return code is
        (>= (byte)'0' and <= (byte)'9') or
        (byte)'+' or (byte)'-' or (byte)'.';
    }

    public static bool IsHex(char code)
    {
        return code is
        (>= '0' and <= '9') or
        (>= 'A' and <= 'F') or
        (>= 'a' and <= 'f');
    }

    public static bool IsAnyFlowSymbol(char code)
    {
        return code is
        ',' or '[' or ']' or '{' or '}';
    }

    public static byte AsHex(char code)
    {
        return code switch
        {
            >= '0' and <= '9' => (byte)(code - (byte)'0'),
            >= 'a' and <= 'f' => (byte)(code - (byte)'a' + 10),
            >= 'A' and <= 'F' => (byte)(code - (byte)'A' + 10),
            _ => throw new InvalidOperationException()
        };
    }
}
