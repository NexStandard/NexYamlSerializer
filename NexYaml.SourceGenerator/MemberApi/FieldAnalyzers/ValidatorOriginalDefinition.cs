using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class ValidatorOriginalDefinition(IMemberSymbolAnalyzer<IFieldSymbol> analyzer, INamedTypeSymbol originalDefinition) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    private static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        return context.Symbol.Type.AllInterfaces.Any(x => x.OriginalDefinition.Equals(originalDefinition, Comparer));
    }
}