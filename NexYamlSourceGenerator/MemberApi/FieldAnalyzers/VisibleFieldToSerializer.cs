using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class VisibleFieldToSerializer(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberContext<IFieldSymbol> context)
    {
        return context.Symbol.DeclaredAccessibility.IsVisibleToEditor(context.DataMemberContext);
    }
}