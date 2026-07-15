
using NexYaml.Core;
using NexYaml.Core.Serialization.Nodes;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Serialization;
public class DelegateWriter(IYamlSerializerResolver resolver, WriteDelegate write) : Writer(resolver)
{
    public override void Write(ReadOnlySpan<char> text)
    {
        write.Invoke(text);
    }

    public override void WriteType<T>(Node context, T? value, DataStyle style) where T : default
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
            return;
        }
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (IYamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            arraySerializer.Write(context, value, style);
            return;
        }
        if (value is IIdentifiable id)
        {
            if (context.Writer.References.Contains(id.Id))
            {
                context.WriteScalar("!!ref ");
                context.WriteScalar(context.Writer.FormatString(context, id.Id.ToString(), style));
                return;
            }
            else
            {
                context.Writer.References.Add(id.Id);
            }
        }
        var type = typeof(T);

        if ((type.IsValueType || type.IsSealed) && !type.IsGenericType)
        {
            Resolver.GetSerializer<T>().Write(context, value, style);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray || type != value.GetType())
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetSerializer(value!.GetType(), typeof(T));
            if (valueType != type)
            {
                context.IsRedirected = true;
            }

            // C# forgets the cast of T when invoking to an object,
            // this way we can call the write method with the "real type"
            // that is in the object
            formatt.Write(context, value, style);
        }
        else
        {
            Resolver.GetSerializer<T>().Write(context, value, style);
        }
    }
}
