using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serialization;

public delegate void WriteDelegate(ReadOnlySpan<char> text);
