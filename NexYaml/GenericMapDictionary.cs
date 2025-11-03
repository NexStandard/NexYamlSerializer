using System.Diagnostics.CodeAnalysis;

namespace NexYaml;

/// <summary>
/// A custom equality comparer for <see cref="Type"/> objects that compares generic type definitions.
/// </summary>
internal class GenericEqualityComparer : IEqualityComparer<Type>
{
    /// <summary>
    /// Compares two <see cref="Type"/> objects for equality, ignoring the specific generic type arguments.
    /// </summary>
    /// <param name="x">The first <see cref="Type"/> to compare.</param>
    /// <param name="y">The second <see cref="Type"/> to compare.</param>
    /// <returns><c>true</c> if the generic type definitions of the two <see cref="Type"/> objects are equal, otherwise <c>false</c>.</returns>
    public bool Equals(Type? x, Type? y)
    {
        if(x is null || y is null)
        {
            return false;
        }
        var thisGenericType = x.IsGenericType ? x.GetGenericTypeDefinition() : x;
        var otherGenericType = y.IsGenericType ? y.GetGenericTypeDefinition() : y;

        return thisGenericType == otherGenericType;
    }

    /// <summary>
    /// Computes the hash code for a <see cref="Type"/>, considering the generic type definition if applicable.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to compute the hash code for.</param>
    /// <returns>The hash code of the given <see cref="Type"/>.</returns>
    public int GetHashCode([DisallowNull] Type type)
    {
        return type.IsGenericType ? type.GetGenericTypeDefinition().GetHashCode() : type.GetHashCode();
    }
}
