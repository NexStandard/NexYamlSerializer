using NexVYaml;
using NexVYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;


public class SimpleClasses
{
    public void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyRecord"/>.
    /// </summary>
    [Fact]
    public void EmptyRecord()
    {
        // Arrange
        Setup();

        // Act
        var originalData = new EmptyRecord();
        var serializedData = YamlSerializer.SerializeToString(originalData);
        var deserializedData = YamlSerializer.Deserialize<EmptyRecord>(serializedData);

        // Assert
        Assert.Equal(originalData, deserializedData);
    }
    /// <summary>
    /// Tests the serialization and deserialization of an <see cref="EmptyClass"/>.
    /// </summary>
    [Fact]
    public void EmptyClass()
    {
        // Arrange
        Setup();

        // Act
        var originalData = new EmptyClass();
        var serializedData = YamlSerializer.SerializeToString(originalData);
        var deserializedData = YamlSerializer.Deserialize<EmptyClass>(serializedData);

        // Assert
        Assert.Equal(originalData, deserializedData);
    }

    /// <summary>
    /// Tests the serialization and deserialization of an EmptyStruct.
    /// </summary>
    [Fact]
    public void EmptyStruct()
    {
        // Arrange
        Setup();

        // Act
        var originalData = new EmptyStruct();
        var serializedData = YamlSerializer.SerializeToString(originalData);
        var deserializedData = YamlSerializer.Deserialize<EmptyStruct>(serializedData);

        // Assert
        Assert.Equal(originalData, deserializedData);
    }
}
