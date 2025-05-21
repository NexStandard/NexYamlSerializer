using NexYaml.Plugins;

namespace NexYaml.Serialization;
class DelegateWriter(IYamlSerializerResolver resolver, IEnumerable<IResolvePlugin> plugins, WriteDelegate write) : Writer(resolver, plugins)
{
    public override void Write(ReadOnlySpan<char> text)
    {
        write.Invoke(text);
    }
}
