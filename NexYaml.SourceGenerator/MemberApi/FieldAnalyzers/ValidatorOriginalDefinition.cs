using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.FieldAnalyzers;

internal class ValidatorOriginalDefinition(IMemberSymbolAnalyzer<IFieldSymbol> analyzer, INamedTypeSymbol originalDefinition) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    private static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        return context.Symbol.Type.AllInterfaces.Any(x => x.OriginalDefinition.Equals(originalDefinition, Comparer));
    }
}