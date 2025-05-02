using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
internal struct TagContext(bool NeedsTag,string Tag)
{
    public bool NeedsTag = NeedsTag;
    public string Tag = Tag;
}
