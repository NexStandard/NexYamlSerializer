using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
internal class WhenNot<T>(
        IMemberSymbolAnalyzer<T> analyzer
        , Func<IMemberSymbolAnalyzer<T>, IMemberSymbolAnalyzer<T>> analyzerTarget
    ) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(MemberContext<T> symbol)
    {
        return !analyzerTarget.Invoke(base._analyzer).AppliesTo(symbol);
    }
}