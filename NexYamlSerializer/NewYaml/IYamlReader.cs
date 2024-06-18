using Stride.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
public interface IYamlReader
{
    public abstract bool IsNull();
    public void Serialize<T>(ref T value, DataMemberMode mode);
    public bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);
}
