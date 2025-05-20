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
    public override void Write<X>(WriteContext<X> context, T? value, DataStyle style)
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
        }
        else
        {
            context.WriteType(value.Value, style);
        }
    }

    public override async ValueTask<T?> Read(IYamlReader stream, ParseContext parseResult)
    {
        return new T?(await stream.Read<T>(parseResult));
    }
}