using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class VisibleFieldToSerializer(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        return context.Symbol.DeclaredAccessibility.IsVisibleToEditor(context.DataMemberContext);
    }
}

internal class HiddenVisibleFieldToSerializer(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        if(context.DataMemberContext.State == DataMemberContextState.Included)
            return context.Symbol.DeclaredAccessibility.IsHiddenVisibleToEditor(context.DataMemberContext);
        return false;
    }
}