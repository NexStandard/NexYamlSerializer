namespace NexYaml.Core;

/// <summary>
/// Represents an exception that is thrown during YAML parsing or processing.
/// </summary>
public class YamlException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YamlException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public YamlException(string message) : base(message)
    {
    }

    public YamlException() : base()
    {
    }

    public YamlException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
