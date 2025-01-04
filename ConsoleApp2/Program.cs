using BenchmarkDotNet.Running;
using NexYaml.Serialization;
using NexYaml;
using Stride.Core;
using System.IO;
using System.Runtime.CompilerServices;
using Test;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
[DataContract]
public partial class Data
{
    [JsonInclude]
    public int Id;
    [JsonInclude]
    public string Value;
}
[DataContract]
public partial class Wrapper
{
    [JsonInclude]
    public Data[] Data;
}


class Program
{

    static void Main()
    {
        NexYamlSerializerRegistry.Init();
        // Calculate how many structs are needed to reach approximately 500MB
        const int structSize = 24;  // Approximate size of each struct in bytes
        const int targetSizeMB = 500;

        int numberOfStructs = (targetSizeMB * 1024 * 1024) / structSize;

        // Create a large list of structs
        var dataList = new List<Data>(10000000);
        for (int i = 0; i < 10000000; i++)
        {
            dataList.Add(new Data { Id = i, Value = "Value " + i });
        }
        var wrapper = new Wrapper();
        wrapper.Data = dataList.ToArray();
        using FileStream stream = new FileStream("D:\\output.json", FileMode.Open);
        using StreamWriter writer = new StreamWriter(stream);
        // Measure the time taken to serialize the data
        Stopwatch stopwatch = Stopwatch.StartNew();
        Yaml.Write(wrapper,writer);
        stopwatch.Stop();
        Console.WriteLine($"Serialization Time: {stopwatch.ElapsedMilliseconds} ms");

        // Optionally, write to a file to see the JSON output
    }
}