﻿using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.PropertyAnalyzers;

internal class ValidatorOriginalDefinition(IMemberSymbolAnalyzer<IPropertySymbol> analyzer, INamedTypeSymbol originalDefinition) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
{
    private static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;

    public override bool AppliesTo(MemberData<IPropertySymbol> context)
    {
        return context.Symbol.Type.AllInterfaces.Any(x => x.OriginalDefinition.Equals(originalDefinition, Comparer));
    }
}