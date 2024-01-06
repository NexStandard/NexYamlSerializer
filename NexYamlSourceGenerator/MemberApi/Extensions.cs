using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.NexAPI;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;

namespace NexYamlSourceGenerator.MemberApi;
public static class Extensionss
{
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
        if (attributeData == null)
            return false;
        return true;
    }
}