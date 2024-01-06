
using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi;
namespace NexYamlSourceGenerator.MemberApi.Analyzers;
internal interface IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(Data<T> symbol);
    SymbolInfo Analyze(Data<T> symbol);
}