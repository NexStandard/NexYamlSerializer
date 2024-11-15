using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class ReferencePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        if (parser.TryGetCurrentTag(out var tag))
        {
            string handle = tag.Handle;
            if (handle == "ref")
            {
                Guid? id = null;
                parser.TryGetScalarAsString(out var idScalar);

                parser.ReadWithVerify(ParseEventType.Scalar);
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
