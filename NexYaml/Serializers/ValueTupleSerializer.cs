using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class ValueTupleSerializer<T1> : YamlSerializer<ValueTuple<T1>>
{
    public override void Write(IYamlWriter stream, ValueTuple<T1> value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref ValueTuple<T1> value, ref ParseResult result)
    {
        var item1 = default(T1);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
        });
        value = new ValueTuple<T1>(item1);
    }
}

public class ValueTupleSerializer<T1, T2> : YamlSerializer<ValueTuple<T1, T2>>
{
    public override void Write(IYamlWriter stream, (T1, T2) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
        });
        value = new ValueTuple<T1, T2>(item1, item2);
    }
}

public class ValueTupleSerializer<T1, T2, T3> : YamlSerializer<ValueTuple<T1, T2, T3>>
{
    public override void Write(IYamlWriter stream, (T1, T2, T3) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2, T3) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
        });
        value = new ValueTuple<T1, T2, T3>(item1, item2, item3);
    }
}

public class ValueTupleSerializer<T1, T2, T3, T4> : YamlSerializer<ValueTuple<T1, T2, T3, T4>>
{
    public override void Write(IYamlWriter stream, (T1, T2, T3, T4) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
            stream.Write(value.Item4, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2, T3, T4) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        var item4 = default(T4);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
            stream.Read(ref item4);
        });
        value = new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }
}

public class ValueTupleSerializer<T1, T2, T3, T4, T5> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5>>
{
    public override void Write(IYamlWriter stream, (T1, T2, T3, T4, T5) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
            stream.Write(value.Item4, DataStyle.Compact);
            stream.Write(value.Item5, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2, T3, T4, T5) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        var item4 = default(T4);
        var item5 = default(T5);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
            stream.Read(ref item4);
            stream.Read(ref item5);
        });
        value = new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
    }
}

public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6>>
{
    public override void Write(IYamlWriter stream, (T1, T2, T3, T4, T5, T6) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
            stream.Write(value.Item4, DataStyle.Compact);
            stream.Write(value.Item5, DataStyle.Compact);
            stream.Write(value.Item6, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2, T3, T4, T5, T6) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        var item4 = default(T4);
        var item5 = default(T5);
        var item6 = default(T6);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
            stream.Read(ref item4);
            stream.Read(ref item5);
            stream.Read(ref item6);
        });
        value = new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
    }
}

public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public override void Write(IYamlWriter stream, (T1, T2, T3, T4, T5, T6, T7) value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
            stream.Write(value.Item4, DataStyle.Compact);
            stream.Write(value.Item5, DataStyle.Compact);
            stream.Write(value.Item6, DataStyle.Compact);
            stream.Write(value.Item7, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref (T1, T2, T3, T4, T5, T6, T7) value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        var item4 = default(T4);
        var item5 = default(T5);
        var item6 = default(T6);
        var item7 = default(T7);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
            stream.Read(ref item4);
            stream.Read(ref item5);
            stream.Read(ref item6);
            stream.Read(ref item7);
        });
        value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
    }
}

public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7, TRest> : YamlSerializer<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    where TRest : struct
{
    public override void Write(IYamlWriter stream, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, DataStyle style)
    {
        stream.WriteSequence(DataStyle.Compact, () =>
        {
            stream.Write(value.Item1, DataStyle.Compact);
            stream.Write(value.Item2, DataStyle.Compact);
            stream.Write(value.Item3, DataStyle.Compact);
            stream.Write(value.Item4, DataStyle.Compact);
            stream.Write(value.Item5, DataStyle.Compact);
            stream.Write(value.Item6, DataStyle.Compact);
            stream.Write(value.Item7, DataStyle.Compact);
            stream.Write(value.Rest, DataStyle.Compact);
        });
    }

    public override void Read(IYamlReader stream, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, ref ParseResult result)
    {
        var item1 = default(T1);
        var item2 = default(T2);
        var item3 = default(T3);
        var item4 = default(T4);
        var item5 = default(T5);
        var item6 = default(T6);
        var item7 = default(T7);
        var item8 = default(TRest);
        stream.ReadSequence(() =>
        {
            stream.Read(ref item1);
            stream.Read(ref item2);
            stream.Read(ref item3);
            stream.Read(ref item4);
            stream.Read(ref item5);
            stream.Read(ref item6);
            stream.Read(ref item7);
            stream.Read(ref item8);
        });
        value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
    }
}
