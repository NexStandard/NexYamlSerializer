using NexYaml.Parser;
using NexYaml.Serialization.Formatters;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class TypePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        if (typeof(T) == typeof(Type))
        {
            object? val = value;
            TypeFormatter.Instance.Read(parser, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }

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
