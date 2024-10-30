using NexVYaml.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter;
public interface IEmitterFactory
{
    public IEmitter Map(EmitState state);
    public IEmitter BeginNodeMap(DataStyle style, bool isSequence);
}
