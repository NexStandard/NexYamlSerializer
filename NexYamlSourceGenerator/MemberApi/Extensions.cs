using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.NexAPI;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;

namespace NexYamlSourceGenerator.MemberApi;

public static class Extensionss
{
    public static bool HasAttribute(this ITypeSymbol symbol, INamedTypeSymbol attribute)
    {
        if (symbol.GetAttributes().Any(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false))
            return true;
        return false;
    }
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
    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attribute)
    {
        if (symbol.GetAttributes().Any(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false))
            return true;
        return false;
    }
    public static string GetFullNamespace(this ITypeSymbol typeSymbol, char separator)
    {
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        var fullNamespace = "";

        while (namespaceSymbol != null && !string.IsNullOrEmpty(namespaceSymbol.Name))
        {
            fullNamespace = namespaceSymbol.Name + separator + fullNamespace;
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        return fullNamespace.TrimEnd(separator);
    }
    internal static IEnumerable<SymbolInfo> AnalyzeAllMembers<T>(this IEnumerable<MemberContext<T>> contexts, params IMemberSymbolAnalyzer<T>[] analyzers)
        where T : ISymbol
    {
        foreach(var context in contexts)
        {
            foreach (var analyzer in analyzers)
            {
                var symbol = analyzer.Analyze(context);
                if(symbol != SymbolInfo.Empty)
                    yield return symbol;
            }
        }
    }
}