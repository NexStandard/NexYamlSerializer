using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;

internal class ArrayPlugin : IResolvePlugin
{
    public bool Write<T, X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            arraySerializer.Write(context, value, style);
            return true;
        }
        return false;
    }
}
