using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ValueTupleSerializer<T1, T2> : YamlSerializer<ValueTuple<T1?, T2?>>
{
    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple2", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        return new ValueTuple<T1?, T2?>(await item1, await item2);
    }
}
public struct ValueTuple2Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,>));
        resolver.RegisterTag("!ValueTuple2", typeof(ValueTuple<,>));
        resolver.Register(this, typeof(ValueTuple<,>), typeof(ValueTuple<,>));
        resolver.Register(this, typeof(ValueTuple<,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3> : YamlSerializer<ValueTuple<T1?, T2?, T3?>>
{
    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple3", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?, T3?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        var item3 = scalarList[2].Read<T3?>(new ParseContext());
        return new ValueTuple<T1?, T2?, T3?>(await item1, await item2, await item3);
    }
}
public struct ValueTuple3Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,,>));
        resolver.RegisterTag("!ValueTuple3", typeof(ValueTuple<,,>));
        resolver.Register(this, typeof(ValueTuple<,,>), typeof(ValueTuple<,,>));
        resolver.Register(this, typeof(ValueTuple<,,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4> : YamlSerializer<ValueTuple<T1?, T2?, T3?, T4?>>
{
    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple4", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?, T3?, T4?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        var item3 = scalarList[2].Read<T3?>(new ParseContext());
        var item4 = scalarList[3].Read<T4?>(new ParseContext());
        return new ValueTuple<T1?, T2?, T3?, T4?>(await item1, await item2, await item3, await item4);
    }
}
public struct ValueTuple4Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,,,>));
        resolver.RegisterTag("!ValueTuple4", typeof(ValueTuple<,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5> : YamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?>>
{
    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple5", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?, T3?, T4?, T5?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        var item3 = scalarList[2].Read<T3?>(new ParseContext());
        var item4 = scalarList[3].Read<T4?>(new ParseContext());
        var item5 = scalarList[4].Read<T5?>(new ParseContext());
        return new ValueTuple<T1?, T2?, T3?, T4?, T5?>(await item1, await item2, await item3, await item4, await item5);
    }
}
public struct ValueTuple5Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,,,,>));
        resolver.RegisterTag("!ValueTuple5", typeof(ValueTuple<,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6> : YamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?>>
{

    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple6", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?, T3?, T4?, T5?, T6?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        var item3 = scalarList[2].Read<T3?>(new ParseContext());
        var item4 = scalarList[3].Read<T4?>(new ParseContext());
        var item5 = scalarList[4].Read<T5?>(new ParseContext());
        var item6 = scalarList[5].Read<T6?>(new ParseContext());
        return new ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?>(await item1, await item2, await item3, await item4, await item5, await item6);
    }
}
public struct ValueTuple6Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,,,,,>));
        resolver.RegisterTag("!ValueTuple6", typeof(ValueTuple<,,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7> : YamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>>
{
    public override void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?> value, DataStyle style)
    {
        context.BeginSequence("!ValueTuple7", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .Write(value.Item7, DataStyle.Compact)
            .End(context);
    }

    public override async ValueTask<(T1?, T2?, T3?, T4?, T5?, T6?, T7?)> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(new ParseContext());
        var item2 = scalarList[1].Read<T2?>(new ParseContext());
        var item3 = scalarList[2].Read<T3?>(new ParseContext());
        var item4 = scalarList[3].Read<T4?>(new ParseContext());
        var item5 = scalarList[4].Read<T5?>(new ParseContext());
        var item6 = scalarList[5].Read<T6?>(new ParseContext());
        var item7 = scalarList[6].Read<T7?>(new ParseContext());
        return new ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>(await item1, await item2, await item3, await item4, await item5, await item6, await item7);
    }
}
public struct ValueTuple7Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(ValueTuple<,,,,,,>));
        resolver.RegisterTag("!ValueTuple7", typeof(ValueTuple<,,,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,>));
        resolver.Register(this, typeof(ValueTuple<,,,,,,>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
