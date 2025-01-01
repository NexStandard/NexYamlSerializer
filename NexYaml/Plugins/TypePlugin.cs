using NexYaml.Parser;
using NexYaml.Serializers;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Plugins;
internal class TypePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (typeof(T) == typeof(Type))
        {
            object? val = value;
            TypeSerializer.Instance.Read(stream, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle provider)
    {
        if (value is Type type)
        {
            TypeSerializer.Instance.Write(stream, type, provider);
            return true;
        }
        return false;
    }
}
