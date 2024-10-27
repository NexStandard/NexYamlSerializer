using NexVYaml;
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Serialization.Formatters;
public class ReferenceFormatter<T> : YamlSerializer<T>
    where T : IIdentifiable
{
    const string refPrefix = "!!ref ";
    protected override void Read(IYamlReader stream, ref T value)
    {
        if(stream.TryGetScalarAsString(out var reference))
        {
            if (reference == null)
                value = default;
            var id = reference.Substring(refPrefix.Length);

        }
    }

    protected override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        stream.Write("!!ref " + value.Id);
    }
}
