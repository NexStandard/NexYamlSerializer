using NexYaml.Core;
using NexYaml.Parser;
using Silk.NET.Maths;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class DelegatePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is Delegate @delegate)
        {
            stream.BeginSequence(style);
            var invocations = @delegate.GetInvocationList();
            foreach (var invocation in invocations)
            {
                if (invocation.Target is IIdentifiable identifiable)
                {
                    stream.Write($"{identifiable.Id}#{invocation.Method.Name}");
                }
            }
            stream.EndSequence();
            return true;
        }
        return false;
    }
}
