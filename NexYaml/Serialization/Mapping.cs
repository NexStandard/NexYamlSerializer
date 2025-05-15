using Stride.Core;

namespace NexYaml.Serialization;

public abstract class Mapping : Node
{
    public abstract WriteContext<Mapping> Write<T>(string key,T value, WriteContext<Mapping> context, DataStyle style);
}
