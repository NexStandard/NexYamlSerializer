﻿using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
internal class HasAttribute<T>(IMemberSymbolAnalyzer<T> analyzer, INamedTypeSymbol attribute) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(MemberData<T> symbol)
    {
        if (symbol.Symbol.TryGetAttribute(attribute, out var _))
        {
            return true;
        }
        return false;
    }
}