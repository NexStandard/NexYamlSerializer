
using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.Analyzers;
internal interface IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(MemberData<T> symbol);
    SymbolInfo Analyze(MemberData<T> symbol);
}