using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class EmptySerializer : IEmitter
{
    public override EmitState State => EmitState.None;

    public EmptySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
{
    }

    public override IEmitter Begin(BeginContext context)
    {
        throw new YamlException($"Can't Begin on {EmitState.None}");
    }

    public override IEmitter WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        return this;
    }

    public override IEmitter End(IEmitter currentEmitter)
    {
        return currentEmitter;
    }
}
