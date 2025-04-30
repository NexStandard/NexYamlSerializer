using NexYaml.Serialization.Emittters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
internal interface IYamlNode
{
    public void Begin(string tag);
    public IEmitter WriteScalar(ReadOnlySpan<char> value);
}
