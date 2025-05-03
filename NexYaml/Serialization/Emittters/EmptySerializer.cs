using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class EmptySerializer : IEmitter
{
    public override EmitState State => EmitState.None;

    public EmptySerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
{
    }

    public override BeginResult Begin(BeginContext context)
    {
        return new BeginResult(this);
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
    }

    public override void End()
    {
    }
}
