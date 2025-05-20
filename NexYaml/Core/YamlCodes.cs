namespace NexYaml.Core;

public static class YamlCodes
{
    public static readonly byte[] YamlDirectiveName = [(byte)'Y', (byte)'A', (byte)'M', (byte)'L'];
    public static readonly byte[] TagDirectiveName = [(byte)'T', (byte)'A', (byte)'G'];

    public static readonly byte[] Bom = [0xFE, 0xFE];
    public static readonly byte[] StreamStart = [(byte)'-', (byte)'-', (byte)'-'];
    public static readonly byte[] DocStart = [(byte)'.', (byte)'.', (byte)'.'];
    public static readonly byte[] CrLf = [Cr, Lf];

    public static readonly byte[] Null0 = [(byte)'!', (byte)'!', (byte)'n', (byte)'u', (byte)'l', (byte)'l'];
    public static readonly string Null = "!!null";

    public const byte Space = (byte)' ';
    public const byte Tab = (byte)'\t';
    public const byte Lf = (byte)'\n';
    public const char NewLine = '\n';
    public const byte Cr = (byte)'\r';
    public const byte Comment = (byte)'#';
    public const byte DirectiveLine = (byte)'%';
    public const byte Alias = (byte)'*';
    public const byte Anchor = (byte)'&';
    public const byte Tag = (byte)'!';
    public const byte SingleQuote = (byte)'\'';
    public const byte DoubleQuote = (byte)'"';
    public const byte LiteralScalerHeader = (byte)'|';
    public const byte FoldedScalerHeader = (byte)'>';
    public const byte Comma = (byte)',';
    public const byte BlockEntryIndent = (byte)'-';
    public const byte ExplicitKeyIndent = (byte)'?';
    public const byte MapValueIndent = (byte)':';
    public const byte FlowMapStart = (byte)'{';
    public const byte FlowMapEnd = (byte)'}';
    public const byte FlowSequenceStart = (byte)'[';
    public const byte FlowSequenceEnd = (byte)']';

    public static bool IsAlphaNumericDashOrUnderscore(byte code)
    {
        return code is
        (>= (byte)'0' and <= (byte)'9') or
        (>= (byte)'A' and <= (byte)'Z') or
        (>= (byte)'a' and <= (byte)'z') or
        (byte)'_' or
        (byte)'-';
    }

    public static bool IsNumber(byte code)
    {
        return code is >= (byte)'0' and <= (byte)'9';
    }

    public static bool IsEmpty(byte code)
    {
        return code is Space or Tab or Lf or Cr;
    }

    public static bool IsLineBreak(byte code)
    {
        return code is Lf or Cr;
    }

    public static bool IsBlank(byte code)
    {
        return code is Space or Tab;
    }

    public static bool IsNumberRepresentation(byte code)
    {
        return code is
        (>= (byte)'0' and <= (byte)'9') or
        (byte)'+' or (byte)'-' or (byte)'.';
    }

    public static bool IsHex(byte code)
    {
        return code is
        (>= (byte)'0' and <= (byte)'9') or
        (>= (byte)'A' and <= (byte)'F') or
        (>= (byte)'a' and <= (byte)'f');
    }

    public static bool IsAnyFlowSymbol(byte code)
    {
        return code is
        (byte)',' or (byte)'[' or (byte)']' or (byte)'{' or (byte)'}';
    }

    public static byte AsHex(byte code)
    {
        return code switch
        {
            >= (byte)'0' and <= (byte)'9' => (byte)(code - (byte)'0'),
            >= (byte)'a' and <= (byte)'f' => (byte)(code - (byte)'a' + 10),
            >= (byte)'A' and <= (byte)'F' => (byte)(code - (byte)'A' + 10),
            _ => throw new InvalidOperationException()
        };
    }
}
