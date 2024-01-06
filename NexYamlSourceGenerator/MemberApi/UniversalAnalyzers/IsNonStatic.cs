using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;

internal class IsNonStatic<T>(IMemberSymbolAnalyzer<T> analyzer) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(Data<T> symbol)
    {
        return !symbol.Symbol.IsStatic;
    }
}