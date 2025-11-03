using System;

namespace NexYamlTest.SimpleClasses;
public record class UnregisteredClass
{
    public int ID { get; set; }
}

internal class UnregisteredBase
{
    public int X;
}

internal class UnregisteredInherited : UnregisteredBase
{
    public double T;
}
internal record class InternalUnregisteredClass
{
    public int ID { get; set; }
}