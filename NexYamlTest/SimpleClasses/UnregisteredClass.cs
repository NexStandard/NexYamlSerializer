﻿using System;

namespace NexYamlTest.SimpleClasses;
public class UnregisteredClass
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

internal class UnregisteredBase
{
    public int X;
}

internal class UnregisteredInherited : UnregisteredBase
{
    public double T;
}
internal class InternalUnregisteredClass
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
        var other = obj as InternalUnregisteredClass;

        // Compare the ID property for equality
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID);
    }
}