using Stride.Core;

namespace NexYaml.Serialization;
public abstract class Sequence : Node
{
    public abstract WriteContext<Sequence> Write<T>(T value, WriteContext<Sequence> context, DataStyle style);
}
