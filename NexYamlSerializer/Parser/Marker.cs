namespace NexVYaml.Parser;

public struct Marker(int position, int line, int col)
{
    public int Position { get; set; } = position;
    public int Line { get; set; } = line;
    public int Col { get; set; } = col;

    public readonly override string ToString()
    {
        return $"Line: {Line}, Col: {Col}, Idx: {Position}";
    }
}

