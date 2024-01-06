using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi;
public static class Extensionss
{
    /// <summary>
    /// Removes all <see cref="SymbolInfo.Empty"/> from the collection.
    /// </summary>
    /// <param name="infos">The collection to filter</param>
    /// <returns>A collection without Empty elements</returns>
    internal static IEnumerable<SymbolInfo> Reduce(this IEnumerable<SymbolInfo> infos)
    {
        foreach (var info in infos)
        {
            if(info != SymbolInfo.Empty)
                yield return info;
        }
    }
    public static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol attribute, out AttributeData attributeData)
    {
        attributeData = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false);
        return attributeData != null;
    }
}