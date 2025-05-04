using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class EmptySerializer : IEmitter
{
    public override EmitState State => EmitState.None;

    public EmptySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
{
    }

    public override EmitResult Begin(BeginContext context)
    {
        throw new YamlException($"Can't Begin on {EmitState.None}");
    }

    public override EmitResult WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        return new EmitResult(this);
    }

    public override EmitResult End(IEmitter currentEmitter)
    {
        return new EmitResult(currentEmitter);
    }
}
