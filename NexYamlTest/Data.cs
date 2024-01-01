using Stride.Core;
namespace NexYamlTest;
[DataContract]
public record EmptyRecord
{
}
[DataContract]
public class EmptyClass
{
    public int ID { get; set; }
    public override bool Equals(object obj)
    {
        // Check if the object is null or of a different type
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        // Convert the object to the same type as this instance
        var other = (EmptyClass)obj;

        // Compare the ID property for equality
        return ID == other.ID;
    }
}
[DataContract]
public struct EmptyStruct
{
    public int ID { get; set; }
    public override bool Equals(object obj)
    {
        // Check if the object is null or of a different type
        if (obj is not EmptyStruct)
        {
            return false;
        }

        // Convert the object to the same type as this instance
        EmptyStruct other = (EmptyStruct)obj;

        // Compare the fields or properties for equality
        return ID == other.ID;
    }

    // Optionally, you may want to override GetHashCode as well when you override Equals
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}
