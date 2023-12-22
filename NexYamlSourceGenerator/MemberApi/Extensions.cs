using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi
{
    public static class Extensionss
    {
        public static bool HasAttribute(this ITypeSymbol symbol, INamedTypeSymbol attribute)
        {
            if (symbol.GetAttributes().Any(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false))
                return true;
            return false;
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
            INamespaceSymbol namespaceSymbol = typeSymbol.ContainingNamespace;
            string fullNamespace = "";

            while (namespaceSymbol != null && !string.IsNullOrEmpty(namespaceSymbol.Name))
            {
                fullNamespace = namespaceSymbol.Name + separator + fullNamespace;
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }

            return fullNamespace.TrimEnd(separator);
        }
    }
}