using NexVYaml.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter.Serializers;
internal interface IEmitter
{
    EmitState State { get; }
    void End();
    void Begin();
    public void BeginScalar(Span<byte> output, ref int offset);
    public void EndScalar(Span<byte> output, ref int offset);
}

