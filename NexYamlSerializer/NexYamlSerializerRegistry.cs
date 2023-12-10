
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using VYaml.Core;
using VYaml.Parser;
using VYaml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace VYaml
{
    public class NexYamlSerializerRegistry : IYamlFormatterResolver
    {
        public static NexYamlSerializerRegistry Instance = new NexYamlSerializerRegistry();
        Dictionary<Type, IYamlFormatter> DefinedFormatters = new Dictionary<Type, IYamlFormatter>()
        {
            // Primitive
            { typeof(short), Int16Formatter.Instance },
            { typeof(int), Int32Formatter.Instance },
            { typeof(long), Int64Formatter.Instance },
            { typeof(ushort), UInt16Formatter.Instance },
            { typeof(uint), UInt32Formatter.Instance },
            { typeof(ulong), UInt64Formatter.Instance },
            { typeof(float), Float32Formatter.Instance },
            { typeof(double), Float64Formatter.Instance },
            { typeof(bool), BooleanFormatter.Instance },
            { typeof(byte), ByteFormatter.Instance },
            { typeof(sbyte), SByteFormatter.Instance },
            { typeof(DateTime), DateTimeFormatter.Instance },
            { typeof(char), CharFormatter.Instance },
            { typeof(byte[]), ByteArrayFormatter.Instance },

            // Nullable Primitive
            { typeof(short?), NullableInt16Formatter.Instance },
            { typeof(int?), NullableInt32Formatter.Instance },
            { typeof(long?), NullableInt64Formatter.Instance },
            { typeof(ushort?), NullableUInt16Formatter.Instance },
            { typeof(uint?), NullableUInt32Formatter.Instance },
            { typeof(ulong?), NullableUInt64Formatter.Instance },
            { typeof(float?), NullableFloat32Formatter.Instance },
            { typeof(double?), NullableFloat64Formatter.Instance },
            { typeof(bool?), NullableBooleanFormatter.Instance },
            { typeof(byte?), NullableByteFormatter.Instance },
            { typeof(sbyte?), NullableSByteFormatter.Instance },
            { typeof(DateTime?), NullableDateTimeFormatter.Instance },
            { typeof(char?), NullableCharFormatter.Instance },

            // StandardClassLibraryFormatter
            { typeof(string), NullableStringFormatter.Instance },
            { typeof(decimal), DecimalFormatter.Instance },
            { typeof(decimal?), new StaticNullableFormatter<decimal>(DecimalFormatter.Instance) },
            { typeof(TimeSpan), TimeSpanFormatter.Instance },
            { typeof(TimeSpan?), new StaticNullableFormatter<TimeSpan>(TimeSpanFormatter.Instance) },
            { typeof(DateTimeOffset), DateTimeOffsetFormatter.Instance },
            { typeof(DateTimeOffset?), new StaticNullableFormatter<DateTimeOffset>(DateTimeOffsetFormatter.Instance) },
            { typeof(Guid), GuidFormatter.Instance },
            { typeof(Guid?), new StaticNullableFormatter<Guid>(GuidFormatter.Instance) },
            { typeof(Uri), UriFormatter.Instance },
        };
        internal NexYamlSerializerRegistry()
        {

        }
        Dictionary<Type, Dictionary<Type, IYamlFormatter>> FormatterBuffer { get; } = new();
        Dictionary<Type,Type> GenericFormatterBuffer { get; } = GenericMapDictionary.Create();
        Dictionary<string, Type> TypeMap = new();
        public IYamlFormatter<T>? GetFormatter<T>()
        {
            if (DefinedFormatters.TryGetValue(typeof(T), out var formatter))
            {
                return (IYamlFormatter<T>)formatter;
            }
            return null;
        }

        public Type GetAliasType(string alias) => TypeMap[alias];

        internal IYamlFormatter<T> CreateGenericFormatter<T>()
        {
            Type t = typeof(T);
            Type genericFormatter = FindGenericFormatter<T>();

            Type genericType = genericFormatter.MakeGenericType(t.GenericTypeArguments);
            return (IYamlFormatter<T>)Activator.CreateInstance(genericType);
        }
        public IYamlFormatter? GetFormatter(Type type)
        {
            if (DefinedFormatters.TryGetValue(type, out IYamlFormatter value))
            {
                return value;
            }

            return null;
        }
        public void RegisterFormatter<T>(IYamlFormatter<T> formatter)
        {
            Type keyType = typeof(T);
            DefinedFormatters[keyType] = formatter;
            TypeMap[keyType.FullName] = keyType;
        }
        public void RegisterFormatter(Type formatter)
        {
            TypeMap[formatter.FullName] = formatter;
        }
        public Type FindGenericFormatter(Type target)
        {
            return GenericFormatterBuffer.FindAssignableType(target);
        }
        public Type FindGenericFormatter<T>()
        {
            return GenericFormatterBuffer.FindAssignableType(typeof(T));
        }
        public void RegisterGenericFormatter(Type target, Type formatterType)
        {
            GenericFormatterBuffer.Add(target, formatterType);
        }
        public void Register<T>(IYamlFormatter<T> formatter, Type interfaceType)
        {
            Type keyType = typeof(T);
            if (!FormatterBuffer.ContainsKey(interfaceType))
            {
                FormatterBuffer.Add(interfaceType, new());
            }
            if (!FormatterBuffer[interfaceType].ContainsKey(keyType))
            {
                FormatterBuffer[interfaceType].Add(keyType, formatter);
            }
            else
            {
                FormatterBuffer[interfaceType][keyType] = formatter;
            }
        }
        public void Register(Type formatterType, Type interfaceType)
        {
            if (!FormatterBuffer.ContainsKey(interfaceType))
            {
                FormatterBuffer.Add(interfaceType, new());
            }
            if (!FormatterBuffer[interfaceType].ContainsKey(formatterType))
            {
                FormatterBuffer[interfaceType].Add(formatterType, null);
            }
            else
            {
                FormatterBuffer[interfaceType][formatterType] = null;
            }
        }
        public IYamlFormatter FindFormatter<T>(Type target)
        {
            if (FormatterBuffer.ContainsKey(typeof(T)))
            {
                if (FormatterBuffer[typeof(T)].TryGetValue(target, out IYamlFormatter value))
                {
                    return FormatterBuffer[typeof(T)][target];
                }
            }
            Type formatterType = FindGenericFormatter(target);
            if (formatterType == null) return null;
            Type closedType = formatterType.MakeGenericType(typeof(T).GenericTypeArguments);
            return (IYamlFormatter)Activator.CreateInstance(closedType);
        }
    }
}
