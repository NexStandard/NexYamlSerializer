using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;

internal class MemberProcessor<T>(IEnumerable<IMemberSymbolAnalyzer<T>> analyzers) : IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(MemberData<T> symbol)
    {
        return analyzers.Any(analyzer => analyzer.AppliesTo(symbol));
    }

    public SymbolInfo Analyze(MemberData<T> symbol)
    {
        if (symbol.DataMemberContext.State == DataMemberContextState.Excluded)
            return SymbolInfo.Empty;

        foreach (var analyzer in analyzers)
        {
            var info = analyzer.Analyze(symbol);
            if (!info.IsEmpty)
                return info;
        }
        return SymbolInfo.Empty;
    }
}