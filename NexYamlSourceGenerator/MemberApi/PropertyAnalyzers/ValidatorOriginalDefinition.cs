using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;

internal class ValidatorOriginalDefinition(IMemberSymbolAnalyzer<IPropertySymbol> analyzer, INamedTypeSymbol originalDefinition) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
{
    private static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    public override bool AppliesTo(Data<IPropertySymbol> context)
    {
        return context.Symbol.Type.AllInterfaces.Any(x => x.OriginalDefinition.Equals(originalDefinition, Comparer));
    }
}