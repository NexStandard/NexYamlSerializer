using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NexVYaml;

static class DictionaryExtension
{
    internal static Type FindAssignableType(this Dictionary<Type, Type> dictionary, Type type)
    {
        return dictionary.TryGetValue(type, out var value) ? value : null;
    }
}

class GenericEqualityComparer : IEqualityComparer<Type>
{
    public bool Equals(Type x, Type y)
    {
        var thisGenericType = x.IsGenericType ? x.GetGenericTypeDefinition() : x;
        var otherGenericType = y.IsGenericType ? y.GetGenericTypeDefinition() : y;

        return thisGenericType == otherGenericType;
    }

    public int GetHashCode([DisallowNull] Type type)
    {
        return type.IsGenericType ? type.GetGenericTypeDefinition().GetHashCode() : type.GetHashCode();
    }
}