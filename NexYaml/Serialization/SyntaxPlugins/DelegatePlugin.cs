﻿using Stride.Core;

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
