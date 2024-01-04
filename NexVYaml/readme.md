# Stride3D YAML Serializer

The `Stride3D.YamlSerializer` is a NuGet package designed to provide YAML serialization support specifically tailored for the Stride3D engine. It offers serialization for interfaces, abstract classes, and generics, making it a versatile tool for your Stride3D projects.

## Features

### Supported Features

1. **Interface Serialization:**
The serializer fully supports the serialization of objects implementing interfaces, allowing you to store and retrieve complex object hierarchies.

2. **Abstract Class Serialization:**
Abstract classes can be serialized without any hassle, providing flexibility in your design patterns.

3. **Generics Support:**
The serializer handles generic types, allowing you to serialize and deserialize objects with generic parameters, when their type is accurately given.

4. **Members** 
The Serializer currently handles public/internal(when tagged with DataMember Attribute) fields and properties ( with get and set/init )

5. **DataMemberIgnore**
The exclusion of members which got tagged with `[Stride.Core.DataMemberIgnore]`.

6. **Generic Restrictions**
The `Stride3D.YamlSerializer` intelligently handles generic restrictions during serialization and deserialization. It respects constraints such as class, struct, new(), and interface constraints on generic parameters. This ensures that the serialization process adheres to the defined constraints, maintaining the integrity and correctness of your generic types.

7. **Structs**
Structs in your Stride3D projects can be easily serialized and deserialized while preserving their value-type characteristics. This feature extends the versatility of the serializer to cover a wide range of data types within your project.

8. **Records**
Efficiently serialize and deserialize record types with the Stride3D.YamlSerializer. This feature ensures seamless integration with records, maintaining their concise, immutable nature. Benefit from precise state representation in your Stride3D projects, optimizing your workflow with minimal effort.

9. **Secure Mode**
Prevents redirection of types during serialization and deserialization processes to ensure that no unknown or unauthorized types can be injected into your code.

### Unsupported Features

1. **Generic Dynamic Deserialization:**
The serializer does not currently support the dynamic deserialization of generic types. Ensure that the deserialization type fits the type in the yaml.

3. **Private Fields:**
The serializer ignores private fields during the serialization process. Make sure to use public/internal properties or fields for data you want to serialize.

4. **DataContract Inherited**
The serializer does not support inherited DataContracts and won't in upcomming releases, classes have to be directly tagged with `[Stride.Core.DataContract]`.

5. **DataStyle**
Compact Mapping of Values isn't supported yet e.g. { X: ... , Y: ... }.

6. **Records with Constructors**
While not currently a primary focus, support for records with constructors is not planned for the immediate future but may be considered in subsequent updates.

7. **Generic Interfaces**
As of now, the serialization of generic Interfaces is not supported and is currently blocked in the source code. Future updates will address this limitation by introducing generic dynamic deserialization, reinstating compatibility with generic interfaces. Stay tuned for upcoming releases that will enhance the Stride3D.YamlSerializer to accommodate this feature.

## Getting Started

Add the Serializer nuget to your csproj.
Add Stride.Engine nuget to your csproj.

1. **Create a class with the DataContract Attribute**

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
