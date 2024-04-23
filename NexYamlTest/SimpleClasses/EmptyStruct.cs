using Stride.Core;
using System;
namespace NexYamlTest.SimpleClasses;

[DataContract]
public struct EmptyStruct
{
    public int ID { get; set; }
    public override readonly bool Equals(object obj)
    {
        // Check if the object is null or of a different type
        if (obj is not EmptyStruct)
        {
            return false;
        }

        // Convert the object to the same type as this instance
        var other = (EmptyStruct)obj;

        // Compare the fields or properties for equality
        return ID == other.ID;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(ID);
    }

    public static bool operator ==(EmptyStruct left, EmptyStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EmptyStruct left, EmptyStruct right)
    {
        return !(left == right);
    }
}
[DataContract]
internal struct InternalEmptyStruct
{
    public int ID { get; set; }
    public override readonly bool Equals(object obj)
    {
        // Check if the object is null or of a different type
        if (obj is not InternalEmptyStruct)
        {
            return false;
        }

        // Convert the object to the same type as this instance
        var other = (InternalEmptyStruct)obj;

        // Compare the fields or properties for equality
        return ID == other.ID;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(ID);
    }

    public static bool operator ==(InternalEmptyStruct left, InternalEmptyStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InternalEmptyStruct left, InternalEmptyStruct right)
    {
        return !(left == right);
    }
}
