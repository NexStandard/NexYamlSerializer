namespace NexYaml.Serialization;

/// <summary>
/// A delegate that writes a span of text to an output.
/// </summary>
/// <param name="text">A read-only span of characters to be written.</param>
public delegate void WriteDelegate(ReadOnlySpan<char> text);
