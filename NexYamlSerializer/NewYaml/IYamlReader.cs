using Stride.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
public interface IYamlReader
{
    bool IsNull();
    void Serialize<T>(ref T value, DataMemberMode mode);
    bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);
}
