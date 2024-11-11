using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class ReferencePlugin : ISyntaxPlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is IIdentifiable id)
        {
            if (stream.References.Contains(id.Id))
            {
                stream.Write($"{stream.Settings.Reference} {id.Id}");
                return true;
            }
            else
            {
                stream.References.Add(id.Id);
                return false;
            }
        }

        return false;
    }
}
