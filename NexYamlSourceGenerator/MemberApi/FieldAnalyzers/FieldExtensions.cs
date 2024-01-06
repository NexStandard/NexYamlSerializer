using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;
internal static class FieldExtensions
{
    internal static IMemberSymbolAnalyzer<IFieldSymbol> HasOriginalDefinition(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer, INamedTypeSymbol originalDefinition)
        => new ValidatorOriginalDefinition(fieldAnalyzer, originalDefinition);
    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsVisibleToSerializer(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
        => new VisibleFieldToSerializer(fieldAnalyzer);
    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsReadOnly(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
        => new ReadOnly(fieldAnalyzer);
    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsArray(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
        => new IsArray(fieldAnalyzer);
}