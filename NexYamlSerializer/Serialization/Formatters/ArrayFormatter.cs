#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class ArrayFormatter<T> : YamlSerializer<T[]>
{
    protected override void Write(IYamlWriter stream, T[] value, DataStyle style)
    {
        stream.WriteSequence(style,() =>
        {
            foreach (var x in value)
            {
                var val = x;
                stream.Write(val, style);
            }
        });
    }

    protected override void Read(IYamlReader stream,  ref T[] value)
    {
        var list = new List<T>();
        stream.ReadSequence(() =>
        {
            var val = default(T);
            stream.Read(ref val);
            list.Add(val!);
        });
        value = [.. list];
    }
}
