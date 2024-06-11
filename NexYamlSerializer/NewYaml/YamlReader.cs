using NexVYaml;
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.NewYaml;
internal class YamlReader(YamlParser parser) : IYamlReader
{
    public bool IsNull()
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return true;
        }
        return false;
    }
    public void Serialize<T>(T value, DataMemberMode mode)
    {

    }
    public void Serialize(ReadOnlySpan<byte> value)
    {
        parser.TryGetScalarAsSpan(out value);
    }
}
