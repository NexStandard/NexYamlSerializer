using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;
using NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYaml.SourceGenerator.MemberApi.PropertyAnalyzers;
internal class HasVisibleSetter(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IPropertySymbol> context)
    {
        if (context.Symbol.SetMethod == null)
            return false;

        return context.Symbol.SetMethod.DeclaredAccessibility.IsVisibleToEditor(context.DataMemberContext);
    }
}
