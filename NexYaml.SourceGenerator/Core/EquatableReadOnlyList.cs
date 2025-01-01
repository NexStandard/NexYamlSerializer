using System.Collections;
namespace NexYaml.SourceGenerator.Core;

public static class EquatableReadOnlyList
{
    public static EquatableReadOnlyList<T> ToEquatableReadOnlyList<T>(this IEnumerable<T> enumerable)
    {
        return new(enumerable.ToArray());
    }
}

/// <summary>
///     A wrapper for IReadOnlyList that provides value equality support for the wrapped list.
/// </summary>

public readonly struct EquatableReadOnlyList<T>(
    IReadOnlyList<T> collection
) : IEquatable<EquatableReadOnlyList<T>>, IReadOnlyList<T>
{
    private IReadOnlyList<T> Collection => collection ?? [];

    public bool Equals(EquatableReadOnlyList<T> other)
    {
        return this.SequenceEqual(other);
    }

    public override bool Equals(object obj)
    {
        return obj is EquatableReadOnlyList<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var item in Collection)
            hashCode.Add(item);

        return hashCode.ToHashCode();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Collection.GetEnumerator();
    }

    public int Count => Collection.Count;
    public T this[int index] => Collection[index];

    public static bool operator ==(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right)
    {
        return !left.Equals(right);
    }
}