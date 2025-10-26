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
        public async Task DoubleQuotedScalar()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "!{[ ] , # ` \" \' &*?|-><=%@. "
            }
            ;
            var s = Yaml.Write(w);
            var d = await TestParser.Read<StringWrapper>(s);
            Assert.NotNull(d);
            Assert.Equal(d.Value, w.Value);
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
        [Fact]
        public async Task LiteralScalar_Compact()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n"
            };
            var s = Yaml.Write(w, DataStyle.Compact);
            var d = await TestParser.Read<StringWrapper>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact]
        public async Task LiteralScalarAtEnd_NoLineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@."
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapper>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact(Skip = "Lf at file end is ignored")]
        public async Task LiteralScalarAtEnd_LineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n"
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapper>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact(Skip = "Lf at file end is ignored")]
        public async Task LiteralScalarAtEnd_TwoLineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapper()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n\n"
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapper>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact]
        public async Task LiteralScalarNormalInYamlMiddle_NoLineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapperSecondProperty()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@."
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapperSecondProperty>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact]
        public async Task LiteralScalarNormalInYamlMiddle_LineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapperSecondProperty()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n"
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapperSecondProperty>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact]
        public async Task LiteralScalarNormalInYamlMiddle_TwoLineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapperSecondProperty()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n\n"
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapperSecondProperty>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
        [Fact]
        public async Task LiteralScalarNormalInYamlMiddle_MultipleLineBreak()
        {
            NexYamlSerializerRegistry.Init();
            var w = new StringWrapperSecondProperty()
            {
                Value = "\n\n!{[ ] \n, # ` \" \' &*?|-><=%@.\n\n\n\n"
            };
            var s = Yaml.Write(w, DataStyle.Normal);
            var d = await TestParser.Read<StringWrapperSecondProperty>(s);
            Assert.NotNull(d);
            Assert.Equal(w.Value, d.Value);
        }
    }
}
