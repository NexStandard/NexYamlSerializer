using NexYaml.Parser;
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

                stream.ReadWithVerify(ParseEventType.Scalar);
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

    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is IIdentifiable id)
        {
            if (stream.References.Contains(id.Id))
            {
                var x = "!!ref " + id.Id;
                stream.WriteScalar(x.AsSpan());
                return true;
            }
            else
            {
                stream.References.Add(id.Id);
                return false;
            }
        }

        return false;
    }
}
