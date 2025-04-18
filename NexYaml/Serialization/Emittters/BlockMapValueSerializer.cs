﻿using NexYaml.Core;

namespace NexYaml.Serialization.Emittters;
internal class BlockMapValueSerializer : IEmitter
{
    public BlockMapValueSerializer(YamlWriter writer, EmitterStateMachine machine) : base(writer, machine)
    {
    }

    public override EmitState State { get; } = EmitState.BlockMappingValue;

    public override void Begin()
    {
        throw new NotSupportedException();
    }

    public override void WriteScalar(ReadOnlySpan<char> output)
    {
        WriteRaw(output);
        WriteNewLine();
        machine.Current = machine.Map(EmitState.BlockMappingKey);
        machine.ElementCount++;
    }

    public override void End()
    {
        // Do nothing
    }
}
