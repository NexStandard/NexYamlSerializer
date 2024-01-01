using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
public class UnregisteredStruct
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
        var other = (EmptyStruct)obj;

        // Compare the fields or properties for equality
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }

    public static bool operator ==(UnregisteredStruct left, UnregisteredStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UnregisteredStruct left, UnregisteredStruct right)
    {
        return !(left == right);
    }
}
internal class InternalUnregisteredStruct
{
    public int ID { get; set; }
    public override bool Equals(object obj)
    {
        // Check if the object is null or of a different type
        if (obj is not InternalUnregisteredStruct)
        {
            return false;
        }

        // Convert the object to the same type as this instance
        var other = (InternalUnregisteredStruct)obj;

        // Compare the fields or properties for equality
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }

    public static bool operator ==(InternalUnregisteredStruct left, InternalUnregisteredStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InternalUnregisteredStruct left, InternalUnregisteredStruct right)
    {
        return !(left == right);
    }
}
