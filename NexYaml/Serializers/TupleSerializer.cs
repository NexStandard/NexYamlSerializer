using System.Collections;
using System.Runtime.CompilerServices;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TupleSerializer<T1, T2> : YamlSerializer<Tuple<T1?, T2?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple2", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1?, T2?>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2);
    }
}
public struct Tuple2Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,>));
        resolver.RegisterTag("Tuple2", typeof(Tuple<,>));
        resolver.Register(this, typeof(Tuple<,>), typeof(Tuple<,>));
        resolver.Register(this, typeof(Tuple<,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3> : YamlSerializer<Tuple<T1?, T2?, T3?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple3", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1?, T2?, T3?>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        stream.Move(ParseEventType.SequenceStart);
        var item1 = stream.Read<T1>(new ParseContext());
        var item2 = stream.Read<T2>(new ParseContext());
        var item3 = stream.Read<T3>(new ParseContext());
        stream.Move(ParseEventType.SequenceEnd);
        return new(await item1, await item2, await item3);
    }
}
public struct Tuple3Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,>));
        resolver.RegisterTag("Tuple3", typeof(Tuple<,,>));
        resolver.Register(this, typeof(Tuple<,,>), typeof(Tuple<,,>));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4> : YamlSerializer<Tuple<T1?, T2?, T3?, T4?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple4", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?>?> Read(IYamlReader stream, ParseContext parseResult)
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
public struct Tuple4Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,>));
        resolver.RegisterTag("Tuple4", typeof(Tuple<,,,>));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(Tuple<,,,>));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4, T5> : YamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple5", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .End(context);

    }

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?>?> Read(IYamlReader stream, ParseContext parseResult)
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

public struct Tuple5Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,>));
        resolver.RegisterTag("Tuple5", typeof(Tuple<,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(Tuple<,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5, T6> : YamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?, T6?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?, T6?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple6", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?>?> Read(IYamlReader stream, ParseContext parseResult)
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

public struct Tuple6Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,,>));
        resolver.RegisterTag("Tuple6", typeof(Tuple<,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>>
{
    public override void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?> value, DataStyle style)
    {
        context.BeginSequence("!Tuple7", style)
            .Write(value!.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .Write(value.Item7, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>?> Read(IYamlReader stream, ParseContext parseResult)
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
public struct Tuple7Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,,,>));
        resolver.RegisterTag("Tuple7", typeof(Tuple<,,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(Tuple<,,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(ITuple));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
