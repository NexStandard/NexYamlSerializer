using Microsoft.CodeAnalysis;

namespace StrideSourceGenerator.NexIncremental
{
    public static class Extensions
    {
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attribute)
        {
            if (symbol.GetAttributes().Any(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false))
            {
                return true;
            }
            return false;
        }
    }
}