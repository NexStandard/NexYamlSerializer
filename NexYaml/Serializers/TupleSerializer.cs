using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TupleSerializer<T1, T2> : YamlSerializer<Tuple<T1, T2>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1, T2>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2);
    }
}

public class TupleSerializer<T1, T2, T3> : YamlSerializer<Tuple<T1, T2, T3>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1, T2, T3>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3);
    }
}

public class TupleSerializer<T1, T2, T3, T4> : YamlSerializer<Tuple<T1, T2, T3, T4>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3, T4>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1, T2, T3, T4>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        var item4 = stream.Read<T4>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3, await item4);
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5> : YamlSerializer<Tuple<T1, T2, T3, T4, T5>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3, T4, T5>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .End(context);
        
    }

    public override async ValueTask<Tuple<T1, T2, T3, T4, T5>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        var item4 = stream.Read<T4>(new ParseContext());
        var item5 = stream.Read<T5>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3, await item4, await item5);
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5, T6> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3, T4, T5, T6>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1, T2, T3, T4, T5, T6>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        var item4 = stream.Read<T4>(new ParseContext());
        var item5 = stream.Read<T5>(new ParseContext());
        var item6 = stream.Read<T6>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3, await item4, await item5, await item6);
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6, T7>?>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3, T4, T5, T6, T7>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .Write(value.Item7, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1, T2, T3, T4, T5, T6, T7>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        var item4 = stream.Read<T4>(new ParseContext());
        var item5 = stream.Read<T5>(new ParseContext());
        var item6 = stream.Read<T6>(new ParseContext());
        var item7 = stream.Read<T7>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3, await item4, await item5, await item6, await item7);
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5, T6, T7, T8> : YamlSerializer<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>?>
    where T8 : notnull
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1, T2, T3, T4, T5, T6, T7, T8>? value, DataStyle style)
    {
        context.BeginSequence("!Tuple1", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .Write(value.Item7, DataStyle.Compact)
            .Write(value.Rest, DataStyle.Compact)
            .End(context);

    }

    public override async ValueTask<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        var item4 = stream.Read<T4>(new ParseContext());
        var item5 = stream.Read<T5>(new ParseContext());
        var item6 = stream.Read<T6>(new ParseContext());
        var item7 = stream.Read<T7>(new ParseContext());
        var item8 = stream.Read<T8>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3, await item4, await item5, await item6, await item7, await item8);
    }
}
