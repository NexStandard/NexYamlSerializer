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
    private static void Compare<T>(T data)
    {
        // Arrange
        Setup();

        // Act
        var serializedData = Yaml.WriteToString(data);
        var deserializedData = Yaml.Read<T>(serializedData);

        // Assert
        Assert.Equal(data, deserializedData);
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyRecord"/>.
    /// </summary>
    [Fact]
    public void EmptyRecord()
    {
        Compare(new EmptyRecord());
    }

    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyClass"/>.
    /// </summary>
    [Fact]
    public void EmptyClass()
    {
        Compare(new EmptyClass());
    }

    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyStruct"/>.
    /// </summary>
    [Fact]
    public void EmptyStruct()
    {
        Compare(new EmptyStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public void InternalEmptyStruct()
    {
        Compare(new InternalEmptyStruct());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public void InternalEmptyClass()
    {
        Compare(new InternalEmptyClass());
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="InternalEmptyStruct"/>.
    /// </summary>
    [Fact]
    public void InternalEmptyRecord()
    {
        Compare(new InternalEmptyRecord());
    }
}
