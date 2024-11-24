using NexYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serializers;
public class NullableSerializer<T> : YamlSerializer<T?> where T : struct
{
    public override void Write(IYamlWriter stream, T? value, DataStyle style)
    {
        if (value is null)
        {
            stream.Write("!!null");
        }
        else
        {
            stream.Write(value.Value, style);
        }
    }

    public override void Read(IYamlReader stream, ref T? value, ref ParseResult result)
    {
        var val = default(T);
        stream.Read(ref val);
        value = new T?(val);
    }
}