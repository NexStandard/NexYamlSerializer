using NexYaml;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class DelegatePlugin : ISyntaxPlugin
{
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
                    stream.Write(identifiable, style);
                }
            }
            stream.EndSequence();
            return true;
        }
        return false;
    }
}
