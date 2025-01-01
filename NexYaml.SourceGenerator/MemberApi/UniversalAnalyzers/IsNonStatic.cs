using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;

internal class IsNonStatic<T>(IMemberSymbolAnalyzer<T> analyzer) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(MemberData<T> symbol)
    {
        return !symbol.Symbol.IsStatic;
    }
}