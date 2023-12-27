using System;
using System.Collections.Generic;
using NexVYaml.Serialization;

namespace NexVYaml;
class FormatterRegistry
{
    internal Dictionary<Type, Dictionary<Type, IYamlFormatter>> FormatterBuffer { get; } = new();
    internal Dictionary<Type, Type> GenericFormatterBuffer { get; } = new Dictionary<Type, Type>(new GenericEqualityComparer())
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
    internal Dictionary<string, Type> TypeMap { get; } = new();
    internal Dictionary<Type, IYamlFormatter> DefinedFormatters { get; } = new Dictionary<Type, IYamlFormatter>()
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
}
