using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class EmptySerializer : IEmitter
{
    public override EmitState State { get; } = EmitState.None;

    public EmptySerializer()  :base(null, null)
    {
    }

    public override void Begin()
    {
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
    }

    public override void End()
    {
    }
}
