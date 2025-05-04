using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;
using NexYaml.Serialization.Emittters;

namespace NexYaml.Serialization;
public readonly record struct BeginContext(bool NeedsTag, string Tag, IEmitter Emitter, IndentationManager Indentation);