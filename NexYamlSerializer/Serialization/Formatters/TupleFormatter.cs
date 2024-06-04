#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;

namespace NexVYaml.Serialization;

public class TupleFormatter<T1> : YamlSerializer<Tuple<T1>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1>(item1!);
    }
}

public class TupleFormatter<T1, T2> : YamlSerializer<Tuple<T1, T2>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1, T2>(item1!, item2!);
    }
}

public class TupleFormatter<T1, T2, T3> : YamlSerializer<Tuple<T1, T2, T3>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
        var item1 = default(T1);
        context.DeserializeWithAlias(ref parser, ref item1);
        var item2 = default(T2);
        context.DeserializeWithAlias(ref parser, ref item2);
        var item3 = default(T3);
        context.DeserializeWithAlias(ref parser, ref item3);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1, T2, T3>(item1, item2, item3);
    }
}

public class TupleFormatter<T1, T2, T3, T4> : YamlSerializer<Tuple<T1, T2, T3, T4>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3, T4>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3, T4>? value)
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
        value = new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }
}

public class TupleFormatter<T1, T2, T3, T4, T5> : YamlSerializer<Tuple<T1, T2, T3, T4, T5>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3, T4, T5>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3, T4, T5>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);
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
        value = new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
    }
}

public class TupleFormatter<T1, T2, T3, T4, T5, T6> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3, T4, T5, T6>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.Write(value.Item6, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3, T4, T5, T6>? value)
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
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
    }
}

public class TupleFormatter<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6, T7>?>
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3, T4, T5, T6, T7>? value, DataStyle style)
    {
        stream.BeginSequence(DataStyle.Compact);
        stream.Write(value!.Item1, style);
        stream.Write(value.Item2, style);
        stream.Write(value.Item3, style);
        stream.Write(value.Item4, style);
        stream.Write(value.Item5, style);
        stream.Write(value.Item6, style);
        stream.Write(value.Item7, style);
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3, T4, T5, T6, T7>? value)
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
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
    }
}

public class TupleFormatter<T1, T2, T3, T4, T5, T6, T7, T8> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>?>
    where T8 : notnull
{
    protected override void Write(IYamlWriter stream, Tuple<T1, T2, T3, T4, T5, T6, T7, T8>? value, DataStyle style)
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

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Tuple<T1, T2, T3, T4, T5, T6, T7, T8>? value)
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
        var item8 = default(T8);
        context.DeserializeWithAlias(ref parser, ref item8);
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(item1, item2, item3, item4, item5, item6, item7, item8);
    }
}
