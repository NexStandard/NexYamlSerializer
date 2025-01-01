using BenchmarkDotNet.Running;
using NexYaml.Serialization;
using NexYaml;
using Stride.Core;
using System.IO;
using System.Runtime.CompilerServices;
using Test;

var script = new MyComponentScript();
var dels = new Delegates2();
dels.Script = script;
NexYamlSerializerRegistry.Init();
var s = Yaml.WriteToString(dels);
Console.WriteLine(s);
var d = Yaml.Read<Delegates2>(s);
// d.Action();


[DataContract]
public class Delegates2 : IIdentifiable
{
    [DataMember]
    public Action Action { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public MyComponentScript Script;
    public void Test()
    {

    }
}
[DataContract]
public class MyComponentScript : IIdentifiable
{
    public Guid Id { get; set; }
    public void ItWorksFinally()
    {
        Console.WriteLine("Delegates are back on the menu boys");
    }
}