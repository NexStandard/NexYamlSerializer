using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serialization;

delegate void WriteDelegate(ReadOnlySpan<char> text);
