﻿using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;
internal class HasVisibleSetter(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IPropertySymbol> context)
    {
        if (context.Symbol.SetMethod == null)
            return false;

        return context.Symbol.SetMethod.DeclaredAccessibility.IsVisibleToEditor(context.DataMemberContext);
    }
}
