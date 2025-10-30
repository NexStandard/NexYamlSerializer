using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml;
using NexYaml.Parser;
using NexYaml.Serialization;

namespace NexYamlTest
{
    internal static class TestParser
    {
        static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
        public static async ValueTask<T?> Read<T>(string s)
        {

            using var reader = new StreamReader(ToStream(s));
            var parser = new YamlParser(reader, IYamlSerializerResolver.Default);
            var pars = parser.Parse();
            var first = pars.First();
            Console.WriteLine(first.Dump());
            reader.BaseStream.Position = 0;
            var f = parser.Parse().First();
            return await f.Read<T>(default(T?));
        }
    }
}
