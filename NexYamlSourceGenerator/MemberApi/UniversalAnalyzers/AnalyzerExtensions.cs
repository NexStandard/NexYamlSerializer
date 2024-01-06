using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
internal static class AnalyzerExtensions
{
    internal static bool IsVisibleToEditor(this Accessibility accessibility, DataMemberContext context)
    {
        if (context.State == DataMemberContextState.Included)
            return accessibility is Accessibility.Public or Accessibility.Internal;
        return accessibility == Accessibility.Public;
    }
    internal static IMemberSymbolAnalyzer<T> WhenNot<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer, Func<IMemberSymbolAnalyzer<T>, IMemberSymbolAnalyzer<T>> analyzerTarget)
        where T : ISymbol
        => new WhenNot<T>(memberAnalyzer, analyzerTarget);
    internal static IMemberSymbolAnalyzer<T> IsNonStatic<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer)
    where T : ISymbol
        => new IsNonStatic<T>(memberAnalyzer);
    internal static IMemberSymbolAnalyzer<T> HasMemberMode<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer,MemberMode mode)
        where T : ISymbol
        => new ValidatorMemberMode<T>(memberAnalyzer,mode);
    public static SymbolInfo Analyze<T>(this IEnumerable<IMemberSymbolAnalyzer<T>> symbols, Data<T> member)
    where T : ISymbol
    {
        return new MemberProcessor<T>(symbols).Analyze(member);
    }
}
