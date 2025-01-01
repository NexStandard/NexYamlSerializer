using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;

namespace NexYaml.SourceGenerator.MemberApi.FieldAnalyzers;
internal static class FieldExtensions
{
    internal static IMemberSymbolAnalyzer<IFieldSymbol> HasOriginalDefinition(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer, INamedTypeSymbol originalDefinition)
    {
        return new ValidatorOriginalDefinition(fieldAnalyzer, originalDefinition);
    }

    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsVisibleToSerializer(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
    {
        return new VisibleFieldToSerializer(fieldAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsReadOnly(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
    {
        return new ReadOnly(fieldAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsArray(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
    {
        return new IsArray(fieldAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<IFieldSymbol> IsConst(this IMemberSymbolAnalyzer<IFieldSymbol> fieldAnalyzer)
    {
        return new ConstFieldAnalyzer(fieldAnalyzer);
    }
}