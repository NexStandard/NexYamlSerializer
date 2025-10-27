using System;
using System.Linq;
using System.Threading.Tasks;
using NexYaml.Parser;
using NexYaml.Serialization;

namespace NexYamlTest
{
    internal static class TestParser
    {
        public static async ValueTask<T?> Read<T>(string s)
        {
            var parser = new YamlParser(s, IYamlSerializerResolver.Default).Parse();
            var first = parser.First();
            Console.WriteLine(first.Dump());
            return await first.Read<T>(new ParseContext());
        }
    }
}
