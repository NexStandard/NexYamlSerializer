namespace NexYaml.Core;

public class YamlException : Exception
{
    public YamlException(string message) : base(message)
    {
    }

    public YamlException(Marker mark, string message) : base($"{message} at {mark}")
    {
    }
}
