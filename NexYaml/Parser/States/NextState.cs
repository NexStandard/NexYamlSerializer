using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal record NextState(ParseEventType CurrentEvent, ParseState State, Scalar? Scalar = null, Tag? Tag = null, Anchor? Anchor = null);
}
