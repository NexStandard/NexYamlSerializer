
using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi;
namespace NexYamlSourceGenerator.MemberApi.Analyzers;
internal abstract class MemberSymbolAnalyzer<T>(IMemberSymbolAnalyzer<T> analyzer) : IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    protected readonly IMemberSymbolAnalyzer<T> _analyzer = analyzer;

    public virtual SymbolInfo Analyze(Data<T> symbol)
    {
        if (AppliesTo(symbol))
            return _analyzer.Analyze(symbol);
        else
            return CreateInfo();
    }
    public abstract bool AppliesTo(Data<T> symbol);
    protected virtual SymbolInfo CreateInfo() => SymbolInfo.Empty;
}
