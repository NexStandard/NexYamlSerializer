using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class RedirectionTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }

    [Fact]
    public async Task InterfaceTester()
    {
        Setup();
        IDInterface dInterface = new Interfacing()
        {
            Id = 10235
        };
        var s = Yaml.Write(dInterface);
        var deserialized = await TestParser.Read<IDInterface>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(dInterface.Id, deserialized.Id);
    }
    [Fact]
    public async Task Redirection_Of_Abstract_Class()
    {
        Setup();
        IDAbstract dInterface = new Abstracting()
        {
            Id = 10235
        };
        var s = Yaml.Write(dInterface);
        var deserialized = await TestParser.Read<IDAbstract>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(dInterface.Id, deserialized.Id);
    }
    [Fact]
    public async Task GenericSimpleTest()
    {
        Setup();
        var generic = new Generics<int>()
        {
            Value = 10235
        };
        var s = Yaml.Write(generic);
        var deserialized = await TestParser.Read<Generics<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(generic.Value, deserialized.Value);
    }
#if NET9_0_OR_GREATER
    [Fact]
    public async Task StackedGenericsStack()
    {
        Setup();
        var generic = new GenericWithRestriction<Generics<int>>()
        {
            Value = new Generics<int>() { Value = 1 }
        };
        var s = Yaml.Write(generic);
        var deserialized = await TestParser.Read<GenericWithRestriction<Generics<int>>>(s);
        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.Value);
        Assert.Equal(generic.Value.Value, deserialized.Value.Value);
    }
#endif
    [Fact]
    public async Task ImplementedGenericsTest()
    {
        Setup();
        var generic = new GenericWithImplementation()
        {
            Value = 43
        };
        var s = Yaml.Write(generic);
        var deserialized = await TestParser.Read<Generics<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(generic.Value, deserialized.Value);
    }
    [Fact]
    public async Task InheritanceTest()
    {
        Setup();
        var generic = new Inherited()
        {
            T = 123,
            X = 1234
        };
        var s = Yaml.Write<Base>(generic);
        var deserialized = await TestParser.Read<Base>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(generic.X, deserialized.X);
    }
}
