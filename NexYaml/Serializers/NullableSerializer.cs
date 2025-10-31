using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

internal class NullableSerializer<T> : IYamlSerializer<T?>
    where T : struct
{
    public void Write<X>(WriteContext<X> context, T? value, DataStyle style) where X : Node
    {
        // do nothing?
    }

    public async ValueTask<T?> Read(Scope scope, T? parseResult)
    {
        if (parseResult.HasValue)
        {
            return await scope.Read(parseResult.Value);
        }
        else
        {
            return await scope.Read<T>();
        }
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
    public IYamlSerializer Instantiate(Type type)
    {
        var gen = typeof(NullableSerializer<>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
