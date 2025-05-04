using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serializers;
public class NullableSerializer<T> : YamlSerializer<T?> where T : struct
{
    public override WriteContext Write(IYamlWriter stream, T? value, DataStyle style, in WriteContext context)
    {
        if (value is null)
        {
            return context.Write(YamlCodes.Null);
        }
        else
        {
            return context.Write(value.Value, style);
        }
    }

    public override void Read(IYamlReader stream, ref T? value, ref ParseResult result)
    {
        var val = default(T);
        stream.Read(ref val);
        value = new T?(val);
    }
}