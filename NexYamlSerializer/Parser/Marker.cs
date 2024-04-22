namespace NexVYaml.Parser;

public struct Marker(int position, int line, int col)
{
    public int Position = position;
    public int Line = line;
    public int Col = col;

    public readonly override string ToString() => $"Line: {Line}, Col: {Col}, Idx: {Position}";
}

