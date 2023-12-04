using System;
using System.Collections.Generic;
using System.Text;
using VYaml.Serialization;

namespace VYaml.Core
{
    class GenericMapDictionary
    {
        private readonly Dictionary<TypeKey, Type> dictionary = new Dictionary<TypeKey, Type>()
        {
            [new TypeKey(typeof(List<>))] = typeof(ListFormatter<>),
            [new TypeKey(typeof(KeyValuePair<,>))] = typeof(KeyValuePairFormatter<,>),
            [new TypeKey(typeof(Dictionary<,>))] = typeof(DictionaryFormatter<,>),
            [new TypeKey(typeof(Tuple<>))] = typeof(TupleFormatter<>),
            [new TypeKey(typeof(Tuple<,>))] = typeof(TupleFormatter<,>),
            [new TypeKey(typeof(Tuple<,,>))] = typeof(TupleFormatter<,,>),
            [new TypeKey(typeof(Tuple<,,,>))] = typeof(TupleFormatter<,,,>),
            [new TypeKey(typeof(Tuple<,,,,>))] = typeof(TupleFormatter<,,,,>),
            [new TypeKey(typeof(Tuple<,,,,,>))] = typeof(TupleFormatter<,,,,,>),
            [new TypeKey(typeof(Tuple<,,,,,,>))] = typeof(TupleFormatter<,,,,,,>),
            [new TypeKey(typeof(Tuple<,,,,,,,>))] = typeof(TupleFormatter<,,,,,,,>)
        };

        public void Add(Type type, Type value)
        {
            dictionary[new TypeKey(type)] = value;
        }
        public Type? FindAssignableType(Type searchType)
        {
            Type value;
            return dictionary.TryGetValue(new TypeKey(searchType), out value) ? value : null;
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
}