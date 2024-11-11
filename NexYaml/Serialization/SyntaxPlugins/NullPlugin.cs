using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;

internal class NullPlugin : ISyntaxPlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is null)
        {
            stream.WriteScalar(stream.Settings.Null);
            return true;
        }
        return false;
    }
}