using Stride.Core;

namespace NexYaml.Serialization.Emittters;
public interface IEmitterFactory
{
    public IEmitter Map(EmitState state);
    public IEmitter BeginNodeMap(DataStyle style, bool isSequence);
}
