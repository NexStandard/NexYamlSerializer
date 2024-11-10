using NexYaml;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
