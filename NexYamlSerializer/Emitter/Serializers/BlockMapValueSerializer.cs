using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockMapValueSerializer(Utf8YamlEmitter emitter) : EmitterSerializer
{
    public override EmitState State { get; } = EmitState.BlockMappingValue;

    public override void Begin()
    {
        throw new NotImplementedException();
    }

    public override void BeginScalar(Span<byte> output, ref int offset)
    {
        // Do nothing
    }

    public override void End()
    {
        // Do nothing
    }

    public override void EndScalar(Span<byte> output, ref int offset)
    {
        output[offset++] = YamlCodes.Lf;
        emitter.StateStack.Current = EmitState.BlockMappingKey;
        emitter.currentElementCount++;
    }
}
