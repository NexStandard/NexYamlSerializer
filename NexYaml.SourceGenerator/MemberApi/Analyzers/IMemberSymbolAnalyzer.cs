
using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Data;
namespace NexYamlSourceGenerator.MemberApi.Analyzers;
internal interface IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(MemberData<T> symbol);
    SymbolInfo Analyze(MemberData<T> symbol);
}