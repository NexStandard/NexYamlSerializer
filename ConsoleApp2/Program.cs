using BenchmarkDotNet.Running;
using NexYaml;
using NexYaml.Serialization;
using Test;

NexYamlSerializerRegistry.Init();
BenchmarkRunner.Run<Benchmarker>();