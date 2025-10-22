using System.Collections.Generic;
using NexYaml.Core;

namespace NexYaml.Parser.States
{
    internal class BlockMapFirstKey(YamlParser parser) : BlockMapKey(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            tokenizer.Read();
            return base.Parse(tokenizer);
        }
    }
}
