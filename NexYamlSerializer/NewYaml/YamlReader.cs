using NexVYaml;
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public void Serialize<T>(ref T value, DataMemberMode mode)
    {

    }
    public void Serialize(ref ReadOnlySpan<byte> value)
    {
        parser.TryGetScalarAsSpan(out value);
    }

    public bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span)
    {
        throw new NotImplementedException();
    }
}
