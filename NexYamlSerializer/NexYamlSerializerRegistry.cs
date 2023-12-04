
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VYaml.Core;
using VYaml.Parser;
using VYaml.Serialization;

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
        Dictionary<Type, Dictionary<Type, IYamlFormatter>> InterfaceBuffer = new();
        Dictionary<Type, Dictionary<Type, IYamlFormatter>> AbstractClassesBuffer = new();
        GenericMapDictionary GenericFormatterBuffer = new();
        Dictionary<string, Type> TypeMap = new();
        List<(Guid,Type,IYamlFormatter)> GenericTemporaryBuffer = new();
        public IYamlFormatter<T>? GetFormatter<T>()
        {
            if (DefinedFormatters.ContainsKey(typeof(T)))
            {
                return (IYamlFormatter<T>)DefinedFormatters[typeof(T)];
            }

            return null;
        }
        public Type GetAliasType(string alias) => TypeMap[alias];
        public Guid RequestGenericBufferStorage(IYamlFormatter formatter,Type formatterTarget)
        {
            Guid guid = Guid.NewGuid();
            if(GenericTemporaryBuffer.Any(x => x.Item2 == formatterTarget))
            {
                return GenericTemporaryBuffer.Where(x => x.Item2 == formatterTarget).First().Item1;
            }
            else
            {
                GenericTemporaryBuffer.Add((guid, formatterTarget, formatter));
            }
            return guid;
        }
        public IYamlFormatter<T> GetGenericBufferedFormatter<T>()
        {
            Type type = typeof(T);
            try
            {
                IYamlFormatter x = GenericTemporaryBuffer.First(x => x.Item2 == type).Item3;
                
                return (IYamlFormatter<T>)x;

            }
            catch
            {
                Type genericFormatter = NexYamlSerializerRegistry.Instance.FindGenericFormatter<T>();
                Type t = typeof(T);
                Type genericType = genericFormatter.MakeGenericType(t.GenericTypeArguments);
                return (IYamlFormatter<T>)Activator.CreateInstance(genericType);
            }
        }
        public void RemoveGenericBuffer(Guid guid)
        {
            GenericTemporaryBuffer.RemoveAll(item => item.Item1 == guid);
        }
        public void ClearGenericBuffer()
        {
            GenericTemporaryBuffer.Clear();
        }
        public IYamlFormatter? GetFormatter(Type type)
        {
            if (DefinedFormatters.ContainsKey(type))
            {
                return DefinedFormatters[type];
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
        public void RegisterInterface<T>(IYamlFormatter<T> formatter, Type interfaceType)
        {
            Type keyType = typeof(T);
            if (!InterfaceBuffer.ContainsKey(interfaceType))
            {
                InterfaceBuffer.Add(interfaceType, new());
            }
            if (!InterfaceBuffer[interfaceType].ContainsKey(keyType))
            {
                InterfaceBuffer[interfaceType].Add(keyType, formatter);
            }
            else
            {
                InterfaceBuffer[interfaceType][keyType] = formatter;
            }
        }
        public void RegisterAbstractClass<T>(IYamlFormatter<T> formatter, Type interfaceType)
        {
            Type keyType = typeof(T);
            if (!AbstractClassesBuffer.ContainsKey(interfaceType))
            {
                AbstractClassesBuffer.Add(interfaceType, new());
            }
            if (!AbstractClassesBuffer[interfaceType].ContainsKey(keyType))
            {
                AbstractClassesBuffer[interfaceType].Add(keyType, formatter);
            }
            else
            {
                AbstractClassesBuffer[interfaceType][keyType] = formatter;
            }
        }
        public IYamlFormatter FindInterfaceFormatter<T>(Tag tag)
        {
            Type type = Type.GetType(tag.Handle);
            return InterfaceBuffer[typeof(T)][type];
        }
        public IYamlFormatter FindInterfaceTypeBased<T>(Type target)
        {
            return InterfaceBuffer[typeof(T)][target];
        }
        public IYamlFormatter FindAbstractTypeBased<T>(Type target)
        {
            return AbstractClassesBuffer[typeof(T)][target];
        }

        public IYamlFormatter FindAbstractFormatter<T>(Tag tag)
        {
            Type type = Type.GetType(tag.Handle);
            return AbstractClassesBuffer[typeof(T)][type];
        }
    }
}
