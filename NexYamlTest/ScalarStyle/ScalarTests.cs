using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml;
using Stride.Core;
using Xunit;

namespace NexYamlTest.ScalarStyle
{
    public class ScalarTests
    {
        [Fact]
        public void PlainScalar()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "ScalarStylePlainScalar",
            };
            var s = Yaml.Write(w);
            Assert.Equal("!NexYamlTest.ScalarStyle.StringWrapper,NexYamlTest\nValue: ScalarStylePlainScalar", s);

        }
        [Fact]
        public void DoubleQuotedScalar()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "!{[ ] , # ` \" \' &*?|-><=%@. "
            }
            ;
            var s = Yaml.Write(w);
            Assert.Contains("\"!{[ ] , # ` \" ' &*?|-><=%@. \"", s);
        }
        [Fact]
        public void LiteralScalar()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@."
            };
            var s = Yaml.Write(w);
            Assert.Contains("|+", s);
        }
        [Fact(Skip = "\\n doesnt get translated to \n")]
        public void LiteralCompactScalar()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@."
            };
            var s = Yaml.Write(w, DataStyle.Compact);
            Assert.Contains("\n\n!{[ ] \n, # ` \" ' &*?|-><=%@.", s);
        }
    }
}
