
using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi;

internal interface IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(MemberContext<T> symbol);
    SymbolInfo Analyze(MemberContext<T> symbol);
}