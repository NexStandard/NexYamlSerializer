﻿using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.ResolvePlugin;

internal class NullPlugin : IResolvePlugin
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

    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (stream.IsNullScalar())
        {
            value = default;
            stream.Move();
            return true;
        }
        return false;
    }
}