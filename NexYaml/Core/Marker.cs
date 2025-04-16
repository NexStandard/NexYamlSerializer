namespace NexYaml.Core;

/// <summary>
/// A position marker within a text stream, tracking line, column, and absolute position.
/// </summary>
/// <param name="position">The absolute character position in the stream.</param>
/// <param name="line">The line number (starting from 0).</param>
/// <param name="col">The column number within the line (starting from 0).</param>
public struct Marker(int position, int line, int col)
{
    /// <summary>
    /// Gets or sets the absolute character position in the stream.
    /// </summary>
    public int Position { get; set; } = position;

    /// <summary>
    /// Gets or sets the line number (starting from 0).
    /// </summary>
    public int Line { get; set; } = line;

    /// <summary>
    /// Gets or sets the column number within the line (starting from 0).
    /// </summary>
    public int Col { get; set; } = col;

    /// <summary>
    /// Returns a string representation of the marker, including line, column, and position.
    /// </summary>
    /// <returns>A formatted string indicating line, column, and position.</returns>
    public override readonly string ToString()
    {
        return $"Line: {Line}, Col: {Col}, Position: {Position}";
    }
}
