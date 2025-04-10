namespace NexYaml.Core;

public class YamlException : Exception
{
    public YamlException(string message) : base(message)
    {
    }

    public YamlException(Marker mark, string message) : base($"{message} at {mark}")
    {
    }
    public static void ThrowExpectedTypeParseException(Type expectedType,string parseFailure, Marker marker)
    {
        throw new YamlException(marker, $"Could not parse: \"{parseFailure}\", expected Type: {expectedType}");
    }
}
