using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;

public class EmptyClassesTest
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
        var deserializedData = await Yaml.ReadAsync<T>(serializedData);

        // Assert
        Assert.Equal(data, deserializedData);
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyRecord"/>.
    /// </summary>
    [Fact]
    public async Task EmptyRecord()
    {
        await Compare(new EmptyRecord());
    }

    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyClass"/>.
    /// </summary>
    [Fact]
    public async Task EmptyClass()
    {
        await Compare(new EmptyClass());
    }

    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyStruct"/>.
    /// </summary>
    [Fact]
    public async Task EmptyStruct()
    {
        await Compare(new EmptyStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public async Task InternalEmptyStruct()
    {
        await Compare(new InternalEmptyStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public async Task InternalEmptyClass()
    {
        await Compare(new InternalEmptyClass());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public async Task InternalEmptyRecord()
    {
        await Compare(new InternalEmptyRecord());
    }
}
