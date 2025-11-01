using System.Collections;
using System.Runtime.CompilerServices;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TupleSerializer<T1, T2> : IYamlSerializer<Tuple<T1?, T2?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple2", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<Tuple<T1?, T2?>> Read(Scope scope, Tuple<T1?, T2?>? parseResult)
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
public struct Tuple2Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,>));
        resolver.RegisterTag("!Tuple2", typeof(Tuple<,>));
        resolver.Register(this, typeof(Tuple<,>), typeof(Tuple<,>));
        resolver.Register(this, typeof(Tuple<,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3> : IYamlSerializer<Tuple<T1?, T2?, T3?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple3", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<Tuple<T1?, T2?, T3?>> Read(Scope scope, Tuple<T1?, T2?, T3?>? parseResult)
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
public struct Tuple3Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,>));
        resolver.RegisterTag("!Tuple3", typeof(Tuple<,,>));
        resolver.Register(this, typeof(Tuple<,,>), typeof(Tuple<,,>));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4> : IYamlSerializer<Tuple<T1?, T2?, T3?, T4?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple4", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<Tuple<T1?, T2?, T3?, T4?>> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?>? parseResult)
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
public struct Tuple4Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,>));
        resolver.RegisterTag("!Tuple4", typeof(Tuple<,,,>));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(Tuple<,,,>));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4, T5> : IYamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple5", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .End(context);

    }

    public async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?>> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?>? parseResult)
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

public struct Tuple5Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,>));
        resolver.RegisterTag("!Tuple5", typeof(Tuple<,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(Tuple<,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}

public class TupleSerializer<T1, T2, T3, T4, T5, T6> : IYamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?, T6?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?, T6?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple6", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?>> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?, T6?>? parseResult)
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

public struct Tuple6Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,,>));
        resolver.RegisterTag("!Tuple6", typeof(Tuple<,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
public class TupleSerializer<T1, T2, T3, T4, T5, T6, T7> : IYamlSerializer<Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>>
{
    public void Write<X>(WriteContext<X> context, Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?> value, DataStyle style) where X : Node
    {
        context.BeginSequence("!Tuple7", style)
            .Write(value.Item1, DataStyle.Compact)
            .Write(value.Item2, DataStyle.Compact)
            .Write(value.Item3, DataStyle.Compact)
            .Write(value.Item4, DataStyle.Compact)
            .Write(value.Item5, DataStyle.Compact)
            .Write(value.Item6, DataStyle.Compact)
            .Write(value.Item7, DataStyle.Compact)
            .End(context);
    }

    public async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>? parseResult)
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
public struct Tuple7Factory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(Tuple<,,,,,,>));
        resolver.RegisterTag("!Tuple7", typeof(Tuple<,,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(Tuple<,,,,,,>));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IStructuralEquatable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IStructuralComparable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(IComparable));
        resolver.Register(this, typeof(Tuple<,,,,,,>), typeof(ITuple));
    }
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
