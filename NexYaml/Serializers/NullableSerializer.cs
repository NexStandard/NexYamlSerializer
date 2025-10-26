using System.IO;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

internal class NullableSerializer<T> : YamlSerializer<Nullable<T>>
    where T : struct
{
    public override void Write<X>(Serialization.WriteContext<X> context, T? value, DataStyle style)
    {
        // do nothing?
    }
    public override async ValueTask<T?> Read(IYamlReader stream, ParseContext parseResult)
    {
        return new T?(await stream.Read<T>(parseResult));
    }
    public override async ValueTask<T?> Read(Scope scope, ParseContext parseResult)
    {
        return new T?(await scope.Read<T>(scope,parseResult));
    }
}
public struct NullableFactory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.RegisterSerializer(typeof(NullableSerializer<>));
        resolver.RegisterTag("Nullable", typeof(Nullable<>));
        resolver.Register(this, typeof(Nullable<>), typeof(Nullable<>));
        resolver.Register(this, typeof(Nullable<>), typeof(System.ValueType));
    }
    public YamlSerializer Instantiate(Type type)
    {
        var gen = typeof(NullableSerializer<>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
