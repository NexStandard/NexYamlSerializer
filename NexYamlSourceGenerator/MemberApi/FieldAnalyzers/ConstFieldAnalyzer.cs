using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;
internal class ConstFieldAnalyzer(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        return context.Symbol.HasConstantValue;
    }
}