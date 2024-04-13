using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;

namespace NexVYaml;

public static class YamlStreamExtensions
{
    public static void WriteTag(this ISerializationWriter stream, string tag)
    {
        stream.SerializeTag(ref tag);
    }
    public static void WriteNull(this ISerializationWriter stream)
    {
        stream.Serialize(YamlCodes.Null0);
    }
}

