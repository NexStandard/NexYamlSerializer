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
        return new EmitResult(this);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
    }

    public override void End()
    {
    }
}
