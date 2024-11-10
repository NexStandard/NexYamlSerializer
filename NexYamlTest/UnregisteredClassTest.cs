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
    private static void Compare<T>(T data)
    {
        // Arrange
        Setup();

        // Act
        var serializedData = YamlSerializer.SerializeToString(data);
        var deserializedData = YamlSerializer.Deserialize<T>(serializedData);

        // Assert
        Assert.Equal(default, deserializedData);
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredClass"/>.
    /// </summary>
    [Fact]
    public void InternalUnregisteredClassTest()
    {
        Compare(new InternalUnregisteredClass());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredStruct"/>.
    /// </summary>
    [Fact]
    public void InternalUnregisteredStructTest()
    {
        Compare(new InternalUnregisteredStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalUnregisteredRecord"/>.
    /// </summary>
    [Fact]
    public void InternalUnregisteredRecordTest()
    {
        Compare(new InternalUnregisteredRecord());
    }
}
