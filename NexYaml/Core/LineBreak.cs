namespace NexYaml.Core;

/// <summary>
/// Represents the type of line break encountered in text.
/// </summary>
public enum LineBreakState
{
    /// <summary>
    /// No line break.
    /// </summary>
    None,

    /// <summary>
    /// Line Feed ('\n') line break.
    /// Common on Unix-based systems.
    /// </summary>
    Lf,

    /// <summary>
    /// Carriage Return followed by Line Feed ("\r\n") line break.
    /// Common on Windows systems.
    /// </summary>
    CrLf,

    /// <summary>
    /// Carriage Return ('\r') line break.
    /// Common on older Macintosh systems.
    /// </summary>
    Cr
}