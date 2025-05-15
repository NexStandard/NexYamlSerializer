using System.IO;
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

    public bool Write<T, X>(WriteContext<X> context, T value, DataStyle style) where X : Node
    {
        if (value is IIdentifiable id)
        {
            if (context.Writer.References.Contains(id.Id))
            {
                context.WriteScalar("!!ref " + id.Id);
                return true;
            }
            else
            {
                context.Writer.References.Add(id.Id);
            }
        }
        return false;
    }
}
