# Stride3D YAML Serializer

The `Stride3D.YamlSerializer` is a NuGet package designed to provide YAML serialization support specifically tailored for the Stride3D engine. It offers serialization for interfaces, abstract classes, and generics, making it a versatile tool for your Stride3D projects.

## Features

### Supported Features

1. **Interface Serialization:**
The serializer fully supports the serialization of objects implementing interfaces, allowing you to store and retrieve complex object hierarchies.
This doesn't work yet with generic classes fully.

2. **Abstract Class Serialization:**
Abstract classes can be serialized without any hassle, providing flexibility in your design patterns.

3. **Generics Support:**
The serializer handles generic types, allowing you to serialize and deserialize objects with generic parameters, when their type is accurately given.

4. **Members** 
The Serializer currently handles public/internal(when tagged with DataMember Attribute) fields and properties ( with get and set/init )

5. **DataMemberIgnore**
The exclusion of members which got tagged with `[Stride.Core.DataMemberIgnore]`.

### Unsupported Features

1. **Generic Dynamic Serialization:**
The serializer does not currently support the dynamic deserialization of generic types. Ensure that the deserialization type fits the type in the yaml.

2. **Structs and Records:**
Serialization for structs and records is not supported in this version. Please use classes or interfaces for serialization.

3. **Private Fields:**
The serializer ignores private fields during the serialization process. Make sure to use public/internal properties or fields for data you want to serialize.

4. **DataContract Inherited**
The serializer does not support inherited DataContracts and won't in upcomming releases, classes have to be directly tagged with `[Stride.Core.DataContract]`.

5. **Generic Restrictions**
Currently Generic Restrictions don't work. A class with a restriction like `where T : ...` will generate a faulty serializer. This will be added in the next release.
## Getting Started

Add the Serializer nuget to your csproj.
Add Stride.Engine nuget to your csproj.

1. **Setup the Serializer**

```csharp
using NexVYaml;

NexYamlSerializerRegistry.Init();
```

2. **Create a class with the DataContract Attribute**

```csharp
using Stride.Core;

namespace ExampleApp;
[DataContract]
public class Data<T>
{
    // Has to be DataMembered as it's internal and wont be viewed unless DataMembered
    [DataMember]
    internal T Value { get; set; }
    public int X2;
    // won't be serialized
    [DataMemberIgnore]
    public int Ignored = 101;
}
```

3. **Create an Instance and Serialize it**

```csharp
using NexVYaml;
using NexVYaml.Serialization;
using ExampleApp;

NexYamlSerializerRegistry.Init();

var serialized = YamlSerializer.SerializeToString(new Data<Data<int>>() {  Value = new Data<int>() { Value = 10 } });
var deserialized = YamlSerializer.Deserialize<Data<Data<int>>>(serialized);
Console.WriteLine(serialized);
Console.WriteLine(deserialized.Value.X2);
```

4. **Create a new class implementing an interface**

```csharp
using Stride.Core;

namespace ExampleApp;
[DataContract]
public class Data2 : Contract
{
    [DataMember]
    public int X2;

    public int Test { get; set; }
}
public interface Contract
{
    int Test {  get; set; }
}
```

4. **Serialize the new class**

```csharp
using NexVYaml;
using NexVYaml.Serialization;
using ExampleApp;

NexYamlSerializerRegistry.Init();

Contract data = new Data2()
{
    Test = 101
};
var serialized = YamlSerializer.SerializeToString(data);
var deserialized = YamlSerializer.Deserialize<Contract>(serialized);
Console.WriteLine(serialized);
Console.WriteLine(deserialized.Test);
```
