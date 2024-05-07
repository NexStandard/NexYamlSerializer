#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;

namespace NexVYaml.Serialization;

public class ValueTupleFormatter<T1> : YamlSerializer<ValueTuple<T1>>
{
    public override void Serialize(ISerializationWriter stream, ValueTuple<T1> value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ValueTuple<T1> value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1>(item1);
    }
}

public class ValueTupleFormatter<T1, T2> : YamlSerializer<ValueTuple<T1, T2>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2>(item1, item2);
    }
}

public class ValueTupleFormatter<T1, T2, T3> : YamlSerializer<ValueTuple<T1, T2, T3>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2, T3) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2, T3) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3>(item1, item2, item3);
    }
}

public class ValueTupleFormatter<T1, T2, T3, T4> : YamlSerializer<ValueTuple<T1, T2, T3, T4>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2, T3, T4) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2, T3, T4) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        var item4 = default(T4);
        context.DeserializeWithAlias(ref parser, ref item4);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }
}

public class ValueTupleFormatter<T1, T2, T3, T4, T5> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2, T3, T4, T5) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2, T3, T4, T5) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        var item4 = default(T4);
        context.DeserializeWithAlias(ref parser, ref item4);
        var item5 = default(T5);
        context.DeserializeWithAlias(ref parser, ref item5);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
    }
}

public class ValueTupleFormatter<T1, T2, T3, T4, T5, T6> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2, T3, T4, T5, T6) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.Write(value.Item6, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2, T3, T4, T5, T6) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        var item4 = default(T4);
        context.DeserializeWithAlias(ref parser, ref item4);
        var item5 = default(T5);
        context.DeserializeWithAlias(ref parser, ref item5);
        var item6 = default(T6);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
    }
}

public class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public override void Serialize(ISerializationWriter stream, (T1, T2, T3, T4, T5, T6, T7) value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.Write(value.Item6, style);
        stream.Write(value.Item7, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref (T1, T2, T3, T4, T5, T6, T7) value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        var item4 = default(T4);
        context.DeserializeWithAlias(ref parser, ref item4);
        var item5 = default(T5);
        context.DeserializeWithAlias(ref parser, ref item5);
        var item6 = default(T6);
        context.DeserializeWithAlias(ref parser, ref item6);
        var item7 = default(T7);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
    }
}

public class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7, TRest> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    where TRest : struct
{
    public override void Serialize(ISerializationWriter stream, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.Write(value.Item6, style);
        stream.Write(value.Item7, style);
        stream.Write(value.Rest, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        var item4 = default(T4);
        context.DeserializeWithAlias(ref parser, ref item4);
        var item5 = default(T5);
        context.DeserializeWithAlias(ref parser, ref item5);
        var item6 = default(T6);
        context.DeserializeWithAlias(ref parser, ref item6);
        var item7 = default(T7);
        context.DeserializeWithAlias(ref parser, ref item7);
        var item8 = default(TRest);
        context.DeserializeWithAlias(ref parser, ref item8);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
    }
}
