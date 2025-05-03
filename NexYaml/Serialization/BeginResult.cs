using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Serialization.Emittters;

namespace NexYaml.Serialization;
internal readonly record struct BeginResult(IEmitter Emitter);