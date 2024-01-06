using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;
internal class HasVisibleGetter(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
{
    public override bool AppliesTo(Data<IPropertySymbol> context)
    {
        if (context.Symbol.GetMethod == null)
            return false;

        return context.Symbol.GetMethod.DeclaredAccessibility.IsVisibleToEditor(context.DataMemberContext);
    }
}
