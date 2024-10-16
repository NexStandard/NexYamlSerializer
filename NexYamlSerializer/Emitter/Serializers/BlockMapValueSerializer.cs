using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockMapValueSerializer(Utf8YamlEmitter emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingValue;

    public void Begin()
    {
        throw new NotImplementedException();
    }

    public void BeginScalar(Span<byte> output, ref int offset)
    {
        // Do nothing
    }

    public void End()
    {
        // Do nothing
    }

    public void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.StateStack.Current = EmitState.BlockMappingKey;
        emitter.currentElementCount++;
    }
}
