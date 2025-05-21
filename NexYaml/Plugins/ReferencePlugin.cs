using System.IO;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;
internal class ReferencePlugin : IResolvePlugin
{
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
    public bool Read<T>(IYamlReader stream, out ValueTask<T?> value, ParseContext result)
    {
        value = default;
        return false;
    }
}
