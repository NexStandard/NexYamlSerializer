using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
internal static class AnalyzerExtensions
{
    internal static bool IsVisibleToEditor(this Accessibility accessibility, DataMemberContext context)
    {
        if (context.State == DataMemberContextState.Included)
            return accessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedAndInternal;
        return accessibility == Accessibility.Public;
    }
    internal static bool IsHiddenVisibleToEditor(this Accessibility accessibility, DataMemberContext context)
    {
        if (context.State == DataMemberContextState.Included)
        {
            return accessibility is
                Accessibility.NotApplicable or
                Accessibility.Private or
                Accessibility.Protected;
        }
        return false;
    }
    internal static IMemberSymbolAnalyzer<T> WhenNot<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer, Func<IMemberSymbolAnalyzer<T>, IMemberSymbolAnalyzer<T>> analyzerTarget)
        where T : ISymbol
    {
        return new WhenNot<T>(memberAnalyzer, analyzerTarget);
    }

    internal static IMemberSymbolAnalyzer<T> IsNonStatic<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer)
        where T : ISymbol
    {
        return new IsNonStatic<T>(memberAnalyzer);
    }

    internal static IMemberSymbolAnalyzer<T> HasMemberMode<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer, MemberMode mode)
            where T : ISymbol
    {
        return new ValidatorMemberMode<T>(memberAnalyzer, mode);
    }

    public static SymbolInfo Analyze<T>(this IEnumerable<IMemberSymbolAnalyzer<T>> symbols, MemberData<T> member)
    where T : ISymbol
    {
        return new MemberProcessor<T>(symbols).Analyze(member);
    }
    public static IMemberSymbolAnalyzer<T> HasAttribute<T>(this IMemberSymbolAnalyzer<T> memberAnalyzer, INamedTypeSymbol attribute)
        where T : ISymbol
    {
        return new HasAttribute<T>(memberAnalyzer, attribute);
    }
}
