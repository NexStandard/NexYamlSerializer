using NexVYaml.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.Emitter;
public static class Utf8StreamExtensions
{
    public static EmitState PreviousState(this IUTF8Stream stream)
    {
        return stream.Previous.State;
    }
    public static IEmitter Map(this IUTF8Stream stream, EmitState state)
    {
        return stream.EmitterFactory.Map(state);
    }
}
