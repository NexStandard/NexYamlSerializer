using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.NexAPI;

namespace NexYamlSourceGenerator.NexIncremental;

internal static class TypeSymbolExtensions
{
    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
    {
        foreach (ISymbol member in type.GetMembers())
        {
            if (member is IPropertySymbol || member is IFieldSymbol)
                yield return member;
        }
    }
}