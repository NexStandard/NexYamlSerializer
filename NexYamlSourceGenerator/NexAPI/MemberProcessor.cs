using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;

namespace NexYamlSourceGenerator.NexAPI;

internal class MemberProcessor<T>(IEnumerable<IMemberSymbolAnalyzer<T>> analyzers) : IMemberSymbolAnalyzer<T>
    where T : ISymbol
{
    public bool AppliesTo(MemberContext<T> symbol)
    {
        foreach(var analyzer in analyzers)
        {
            return analyzer.AppliesTo(symbol);
        }
        return false;
    }

    public SymbolInfo Analyze(MemberContext<T> symbol)
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
internal static class CollectionAnalyzers
{
    public static SymbolInfo Analyze<T>(this IEnumerable<IMemberSymbolAnalyzer<T>> symbols, MemberContext<T> member)
        where T : ISymbol
    {
        return new MemberProcessor<T>(symbols).Analyze(member);
    }
}