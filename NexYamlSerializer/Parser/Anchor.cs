using System;

namespace NexVYaml.Parser;

public class Anchor(string name, int id) : IEquatable<Anchor>
{
    public string Name { get; } = name;
    public int Id { get; } = id;

    public bool Equals(Anchor? other)
    {
        return other != null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Anchor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public override string ToString()
    {
        return $"{Name} Id={Id}";
    }
}
