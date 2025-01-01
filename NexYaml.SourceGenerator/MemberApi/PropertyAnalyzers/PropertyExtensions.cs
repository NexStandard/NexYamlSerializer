using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;

namespace NexYaml.SourceGenerator.MemberApi.PropertyAnalyzers;
internal static class PropertyExtensions
{
    internal static IMemberSymbolAnalyzer<IPropertySymbol> HasOriginalDefinition(this IMemberSymbolAnalyzer<IPropertySymbol> propertySymbolAnalyzer, INamedTypeSymbol originalDefinition)
    {
        return new ValidatorOriginalDefinition(propertySymbolAnalyzer, originalDefinition);
    }

    internal static IMemberSymbolAnalyzer<IPropertySymbol> HasVisibleGetter(this IMemberSymbolAnalyzer<IPropertySymbol> propertySymbolAnalyzer)
    {
        return new HasVisibleGetter(propertySymbolAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<IPropertySymbol> HasVisibleSetter(this IMemberSymbolAnalyzer<IPropertySymbol> propertySymbolAnalyzer)
    {
        return new HasVisibleSetter(propertySymbolAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<IPropertySymbol> IsArray(this IMemberSymbolAnalyzer<IPropertySymbol> propertySymbolAnalyzer)
    {
        return new IsArray(propertySymbolAnalyzer);
    }
}
