using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;
internal class ReferencePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (stream.TryGetCurrentTag(out var tag))
        {
            var handle = tag.Handle;

            if (handle == "ref")
            {
                Guid? id = null;
                stream.TryGetScalarAsString(out var idScalar);

                stream.Move(ParseEventType.Scalar);
                if (idScalar != null)
                {
                    result.IsReference = true;
                    result.Reference = Guid.Parse(idScalar);
                }
                return true;
            }
        }
        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle style, WriteContext context, out WriteContext newContext)
    {
        if (value is IIdentifiable id)
        {
            if (stream.References.Contains(id.Id))
            {
                newContext = context.WriteScalar("!!ref " + id.Id);
                return true;
            }
            else
            {
                stream.References.Add(id.Id);
            }
        }
        newContext = default;
        return false;
    }
}
