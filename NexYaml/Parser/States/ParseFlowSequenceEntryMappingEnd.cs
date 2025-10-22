using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class ParseFlowSequenceEntryMappingEnd(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            return new(ParseEventType.MappingEnd,ParseState.FlowSequenceEntry);
        }
    }
}
