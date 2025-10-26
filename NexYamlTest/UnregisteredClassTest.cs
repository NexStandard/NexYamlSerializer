using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class UnregisteredClassTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    private static async Task Compare<T>(T data)
    {
        // Arrange
        Setup();

        // Act
        var serializedData = Yaml.Write(data);
        var deserializedData = await TestParser.Read<T>(serializedData);

        // Assert
        Assert.Equal(default, deserializedData);
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredClass"/>.
    /// </summary>
    [Fact]
    public async Task InternalUnregisteredClassTest()
    {
        await Compare(new InternalUnregisteredClass());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredStruct"/>.
    /// </summary>
    [Fact]
    public async Task InternalUnregisteredStructTest()
    {
        await Compare(new InternalUnregisteredStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredRecord"/>.
    /// </summary>
    [Fact]
    public async Task InternalUnregisteredRecordTest()
    {
        await Compare(new InternalUnregisteredRecord());
    }
}
