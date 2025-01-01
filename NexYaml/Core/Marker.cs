namespace NexYaml.Core;

public struct Marker(int position, int line, int col)
{
    public int Position { get; set; } = position;
    public int Line { get; set; } = line;
    public int Col { get; set; } = col;

    public override readonly string ToString()
    {
        return $"Line: {Line}, Col: {Col}, Idx: {Position}";
    }
}

