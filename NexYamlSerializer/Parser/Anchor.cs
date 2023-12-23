#nullable enable
using System;

namespace NexVYaml.Parser
{
    public class Anchor(string name, int id) : IEquatable<Anchor>
    {
        public string Name { get; } = name;
        public int Id { get; } = id;

        public bool Equals(Anchor? other) => other != null && Id == other.Id;
        public override bool Equals(object? obj) => obj is Anchor other && Equals(other);
        public override int GetHashCode() => Id;

        public override string ToString() => $"{Name} Id={Id}";
    }
}
