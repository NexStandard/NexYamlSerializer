﻿namespace NexYaml.Serialization;
internal static class SerializerExtensions
{
    internal static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(bool) ||
               type == typeof(byte) ||
               type == typeof(sbyte) ||
               type == typeof(char) ||
               type == typeof(short) ||
               type == typeof(ushort) ||
               type == typeof(int) ||
               type == typeof(uint) ||
               type == typeof(long) ||
               type == typeof(ulong) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(decimal) ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(Type) ||
               type == typeof(TimeSpan);
    }
}
