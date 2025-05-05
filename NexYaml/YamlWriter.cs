using NexYaml.Core;
using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;

namespace NexYaml;

public class YamlWriter : IYamlWriter
{
    public HashSet<Guid> References { get; private set; } = new();
    public List<IResolvePlugin> Plugins { get; private set; } 
    public IYamlSerializerResolver Resolver { get; init; }

    internal StyleEnforcer enforcer = new();
    private readonly StreamWriter writer;
    internal EmitterStateMachine StateMachine { get; }

    public YamlWriter(StreamWriter writer,IYamlSerializerResolver resolver, List<IResolvePlugin> plugins)
    {
        StateMachine = new EmitterStateMachine(this);
        Plugins = plugins;
        this.writer = writer;
        Resolver = resolver;
    }

    public void WriteRaw(ReadOnlySpan<char> value)
    {
        writer.Write(value);
    }
    public void WriteRaw(string value)
    {
        writer.Write(value);
    }
    public void WriteRaw(char value)
    {
        writer.Write(value);
    }
}

