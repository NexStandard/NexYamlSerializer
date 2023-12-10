using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using VYaml.Serialization;

namespace VYaml.Core
{
    class GenericMapDictionary
    {
        public static Dictionary<Type,Type> Create()
        {
            return new Dictionary<Type, Type>(new GenericEqualityComparer())
            {
                [typeof(List<>)] = typeof(ListFormatter<>),
                [typeof(KeyValuePair<,>)] = typeof(KeyValuePairFormatter<,>),
                [typeof(Dictionary<,>)] = typeof(DictionaryFormatter<,>),
                [typeof(Tuple<>)] = typeof(TupleFormatter<>),
                [typeof(Tuple<,>)] = typeof(TupleFormatter<,>),
                [typeof(Tuple<,,>)] = typeof(TupleFormatter<,,>),
                [typeof(Tuple<,,,>)] = typeof(TupleFormatter<,,,>),
                [typeof(Tuple<,,,,>)] = typeof(TupleFormatter<,,,,>),
                [typeof(Tuple<,,,,,>)] = typeof(TupleFormatter<,,,,,>),
                [typeof(Tuple<,,,,,,>)] = typeof(TupleFormatter<,,,,,,>),
                [typeof(Tuple<,,,,,,,>)] = typeof(TupleFormatter<,,,,,,,>)
            };
        }
    }
    internal static class DictionaryExtension
    {
        internal static Type FindAssignableType(this Dictionary<Type, Type> dictionary, Type type)
        {
            return dictionary.TryGetValue(type, out var value) ? value : null;
        }
    }

    class TypeKey
    {
        private readonly Type type;

        public TypeKey(Type type)
        {
            this.type = type;
        }

        public override int GetHashCode()
        {
            // Use the generic type definition's hash code
            return type.IsGenericType ? type.GetGenericTypeDefinition().GetHashCode() : type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeKey otherKey)
            {
                Type otherType = otherKey.type;
                Type thisGenericType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                Type otherGenericType = otherType.IsGenericType ? otherType.GetGenericTypeDefinition() : otherType;

                return thisGenericType == otherGenericType;
            }
            return false;
        }
    }
    class GenericEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            Type thisGenericType = x.IsGenericType ? x.GetGenericTypeDefinition() : x;
            Type otherGenericType = y.IsGenericType ? y.GetGenericTypeDefinition() : y;

            return thisGenericType == otherGenericType;
        }

        public int GetHashCode([DisallowNull] Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition().GetHashCode() : type.GetHashCode();
        }
    }
}