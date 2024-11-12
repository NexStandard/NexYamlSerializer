using NexYaml.Serialization.Formatters;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class TypePlugin : ISyntaxPlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle provider)
    {
        if(value is Type type)
        {
            TypeFormatter.Instance.Write(stream, type, provider);
            return true;
        }
        return false;
    }
}
