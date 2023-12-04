using Microsoft.CodeAnalysis;

namespace StrideSourceGenerator.Core
{
    internal static class WellKnownReferences
    {
        public static INamedTypeSymbol DataMemberAttribute(Compilation compilation)
        {
            return compilation.GetTypeByMetadataName("System.Runtime.Serialization.DataMemberAttribute");
        }

        public static INamedTypeSymbol DataMemberIgnoreAttribute(Compilation compilation)
        {
            return compilation.GetTypeByMetadataName("System.Runtime.Serialization.IgnoreDataMemberAttribute");
        }
        public static INamedTypeSymbol DataContractAttribute(Compilation compilation)
        {
            return compilation.GetTypeByMetadataName("System.Runtime.Serialization.DataContractAttribute");
        }
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attribute)
        {
            if (symbol.GetAttributes().Any(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false))
                return true;
            return false;
        }
    }
}