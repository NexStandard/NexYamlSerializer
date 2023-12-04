using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.Implementations;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;

internal static class AnalyzerExtensions
{
    internal static bool IsVisibleToEditor(this Accessibility accessibility, DataMemberContext context)
    {
        if (context.Exists)
            return accessibility == Accessibility.Public || accessibility == Accessibility.Internal;
        return accessibility == Accessibility.Public;
    }
    internal static IMemberSymbolAnalyzer<T> WhenNot<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer, Func<IMemberSymbolAnalyzer<T>, IMemberSymbolAnalyzer<T>> analyzerTarget)
        where T : ISymbol
        => new WhenNot<T>(memberAnalyzer, analyzerTarget);
    internal static IMemberSymbolAnalyzer<T> IsNonStatic<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer)
    where T : ISymbol
        => new IsNonStatic<T>(memberAnalyzer);
}
