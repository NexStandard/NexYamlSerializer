using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class BlockSequenceFirstEntry(YamlParser parser) : BlockSequenceEntry(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            tokenizer.Read();
            return base.Parse(tokenizer);
        }
    }
}
