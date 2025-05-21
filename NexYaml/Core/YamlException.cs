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

    /// <summary>
    /// Initializes a new instance of the <see cref="YamlException"/> class with the specified error message and marker.
    /// The marker provides the location where the exception occurred in the YAML document.
    /// </summary>
    /// <param name="mark">The location in the document where the exception occurred.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public YamlException(Marker mark, string message) : base($"{message} at {mark}")
    {
    }

    public YamlException() : base()
    {
    }

    public YamlException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Throws a <see cref="YamlException"/> indicating a failure to parse a value, 
    /// with the expected type and the actual value that failed to parse.
    /// </summary>
    /// <param name="expectedType">The expected type that was supposed to be parsed.</param>
    /// <param name="parseFailure">The value that could not be parsed.</param>
    /// <param name="marker">The location in the YAML document where the failure occurred.</param>
    public static YamlException ThrowExpectedTypeParseException(Type expectedType, string? parseFailure, Marker marker)
    {
        return new YamlException(marker, $"Could not parse: \"{parseFailure}\", expected Type: {expectedType}");
    }
}