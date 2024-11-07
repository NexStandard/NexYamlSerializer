using NexVYaml.Emitter;
using NexYaml.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal class BlockMapValueSerializer(UTF8Stream emitter) : IEmitter
{
    public EmitState State { get; } = EmitState.BlockMappingValue;

    public void Begin()
    {
        throw new NotImplementedException();
    }

    public void BeginScalar(Span<byte> output)
    {
        // Do nothing
    }

    public void End()
    {
        // Do nothing
    }

    public void EndScalar()
    {
        emitter.WriteRaw(YamlCodes.Lf);
        emitter.Current = emitter.Map(EmitState.BlockMappingKey);
        emitter.currentElementCount++;
    }
}
