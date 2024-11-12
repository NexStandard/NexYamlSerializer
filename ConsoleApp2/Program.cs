using BenchmarkDotNet.Running;
using NexYaml;
using NexYaml.Serialization;
using Test;
var s = new Benchmarker();
s.Setup();
s.Yaml();