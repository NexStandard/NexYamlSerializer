using NexVYaml.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexYamlSerializer.Emitter;
public interface IEmitterFactory
{
    public IEmitter Map(EmitState state);
    public IEmitter BeginNodeMap(DataStyle style, bool isSequence);
}
