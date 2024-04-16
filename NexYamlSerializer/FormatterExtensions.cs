using NexVYaml;
using System;
using System.Linq;

namespace NexYamlSerializer;
internal static class FormatterExtensions
{
    internal static bool IsPrimtiveType(Type type)
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
               type == typeof(TimeSpan);
    }
    internal static bool IsPrimitiveType(this IYamlFormatter formatter, Type type)
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
               type == typeof(TimeSpan);
    }
}
