using NexVYaml;
using NexVYaml.Serialization;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class UnregisteredClassTest
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    private void Compare<T>(T data)
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
