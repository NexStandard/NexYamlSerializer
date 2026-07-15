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
            Console.WriteLine(s);
            using var reader = new StreamReader(ToStream(s));
            var parser = new NewYamlParser(reader, NexYamlSerializerRegistry.Instance);
            var pars = parser;
            foreach(var first in parser)
            {
                Console.WriteLine(first.Data.Dump());
                break;
            }
            reader.BaseStream.Position = 0;
            var parser2= new NewYamlParser(reader, NexYamlSerializerRegistry.Instance);
            foreach(var second in parser2)
            {
                return await second.Read<T>();
            }
            throw new Exception();
        }
    }
}
