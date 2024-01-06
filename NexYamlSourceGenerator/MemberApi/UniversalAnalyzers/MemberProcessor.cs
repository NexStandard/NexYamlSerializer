using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;

internal class MemberProcessor<T>(IEnumerable<IMemberSymbolAnalyzer<T>> analyzers) : IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(Data<T> symbol)
    {
        foreach (var analyzer in analyzers)
        {
            return analyzer.AppliesTo(symbol);
        }
        return false;
    }

    public SymbolInfo Analyze(Data<T> symbol)
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