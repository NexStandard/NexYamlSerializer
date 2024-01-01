using Stride.Core;
using System;
namespace NexYamlTest.SimpleClasses;

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
        var other = obj as EmptyClass;

        // Compare the ID property for equality
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }
}
[DataContract]
internal class InternalEmptyClass
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
        var other = obj as InternalEmptyClass;

        // Compare the ID property for equality
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }
}
