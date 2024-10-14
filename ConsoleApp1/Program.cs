using BenchmarkDotNet.Running;
using NexVYaml;
using NexYamlTest.SimpleClasses;
using Silk.NET.OpenGL;
NexYamlSerializerRegistry.Init();

BenchmarkRunner.Run<BenchmarkSerialization>();