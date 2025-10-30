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

    public override async ValueTask<Tuple<T1?, T2?>?> Read(Scope scope, Tuple<T1?, T2?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
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

    public override async ValueTask<Tuple<T1?, T2?, T3?>?> Read(Scope scope, Tuple<T1?, T2?, T3?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
        var item3 = scalarList[2].Read<T3?>(default);
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

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?>?> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
        var item3 = scalarList[2].Read<T3?>(default);
        var item4 = scalarList[3].Read<T4?>(default);
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

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?>?> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
        var item3 = scalarList[2].Read<T3?>(default);
        var item4 = scalarList[3].Read<T4?>(default);
        var item5 = scalarList[4].Read<T5?>(default);
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

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?>?> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?, T6?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
        var item3 = scalarList[2].Read<T3?>(default);
        var item4 = scalarList[3].Read<T4?>(default);
        var item5 = scalarList[4].Read<T5?>(default);
        var item6 = scalarList[5].Read<T6?>(default);
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

    public override async ValueTask<Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>?> Read(Scope scope, Tuple<T1?, T2?, T3?, T4?, T5?, T6?, T7?>? parseResult)
    {
        var scalarScope = scope.As<SequenceScope>();
        var scalarList = scalarScope.ToList();
        var item1 = scalarList[0].Read<T1?>(default);
        var item2 = scalarList[1].Read<T2?>(default);
        var item3 = scalarList[2].Read<T3?>(default);
        var item4 = scalarList[3].Read<T4?>(default);
        var item5 = scalarList[4].Read<T5?>(default);
        var item6 = scalarList[5].Read<T6?>(default);
        var item7 = scalarList[6].Read<T7?>(default);
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
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(TupleSerializer<,,,,,,>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
