using NexYaml;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.Enums
{
    public class EnumTest
    {
        [Fact]
        public async Task ReadWriteEnum()
        {
            NexYamlSerializerRegistry.Init();
            var x = new Generics<TestEnums>();
            x.Value = TestEnums.ThirdValue;
            var s = Yaml.Write(x);
            var d = await TestParser.Read<Generics<TestEnums>>(s);
            Assert.NotNull(d);
            Assert.Equal(TestEnums.ThirdValue, d.Value);
        }
    }
}
