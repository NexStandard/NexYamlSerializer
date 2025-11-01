using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ValueTupleSerializer<T1, T2> : IYamlSerializer<ValueTuple<T1?, T2?>>
{
    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!ValueTuple2", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<(T1?, T2?)> Read(Scope scope, (T1?, T2?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            i++;
        }
        return new(await item1, await item2);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3> : IYamlSerializer<ValueTuple<T1?, T2?, T3?>>
{
    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!ValueTuple3", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<(T1?, T2?, T3?)> Read(Scope scope, (T1?, T2?, T3?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        ValueTask<T3?> item3 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            else if (i == 2)
                item3 = subscope.Read<T3?>();
            i++;
        }
        return new(await item1, await item2, await item3);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4> : IYamlSerializer<ValueTuple<T1?, T2?, T3?, T4?>>
{
    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!ValueTuple4", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<(T1?, T2?, T3?, T4?)> Read(Scope scope, (T1?, T2?, T3?, T4?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        ValueTask<T3?> item3 = default;
        ValueTask<T4?> item4 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            else if (i == 2)
                item3 = subscope.Read<T3?>();
            else if (i == 3)
                item4 = subscope.Read<T4?>();
            i++;
        }
        return new(await item1, await item2, await item3, await item4);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5> : IYamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?>>
{
    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!ValueTuple5", DataStyle.Compact)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<(T1?, T2?, T3?, T4?, T5?)> Read(Scope scope, (T1?, T2?, T3?, T4?, T5?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        ValueTask<T3?> item3 = default;
        ValueTask<T4?> item4 = default;
        ValueTask<T5?> item5 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            else if (i == 2)
                item3 = subscope.Read<T3?>();
            else if (i == 3)
                item4 = subscope.Read<T4?>();
            else if (i == 4)
                item5 = subscope.Read<T5?>();
            i++;
        }
        return new(await item1, await item2, await item3, await item4, await item5);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6> : IYamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?>>
{

    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?> value, DataStyle style) where X : Node
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

    public async ValueTask<(T1?, T2?, T3?, T4?, T5?, T6?)> Read(Scope scope, (T1?, T2?, T3?, T4?, T5?, T6?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        ValueTask<T3?> item3 = default;
        ValueTask<T4?> item4 = default;
        ValueTask<T5?> item5 = default;
        ValueTask<T6?> item6 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            else if (i == 2)
                item3 = subscope.Read<T3?>();
            else if (i == 3)
                item4 = subscope.Read<T4?>();
            else if (i == 4)
                item5 = subscope.Read<T5?>();
            else if (i == 5)
                item6 = subscope.Read<T6?>();
            i++;
        }
        return new(await item1, await item2, await item3, await item4, await item5, await item6);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7> : IYamlSerializer<ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>>
{
    public void Write<X>(WriteContext<X> context, ValueTuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?> value, DataStyle style) where X : Node
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

    public async ValueTask<(T1?, T2?, T3?, T4?, T5?, T6?, T7?)> Read(Scope scope, (T1?, T2?, T3?, T4?, T5?, T6?, T7?) parseResult)
    {
        ValueTask<T1?> item1 = default;
        ValueTask<T2?> item2 = default;
        ValueTask<T3?> item3 = default;
        ValueTask<T4?> item4 = default;
        ValueTask<T5?> item5 = default;
        ValueTask<T6?> item6 = default;
        ValueTask<T7?> item7 = default;
        int i = 0;

        foreach (var subscope in scope.As<SequenceScope>())
        {
            if (i == 0)
                item1 = subscope.Read<T1?>();
            else if (i == 1)
                item2 = subscope.Read<T2?>();
            else if (i == 2)
                item3 = subscope.Read<T3?>();
            else if (i == 3)
                item4 = subscope.Read<T4?>();
            else if (i == 4)
                item5 = subscope.Read<T5?>();
            else if (i == 5)
                item6 = subscope.Read<T6?>();
            else if (i == 6)
                item7 = subscope.Read<T7?>();
            i++;
        }
        return new(await item1, await item2, await item3, await item4, await item5, await item6, await item7);
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(ValueTupleSerializer<,,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
