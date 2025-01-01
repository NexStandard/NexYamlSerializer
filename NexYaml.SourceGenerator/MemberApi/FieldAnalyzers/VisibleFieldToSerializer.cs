using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;
using NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYaml.SourceGenerator.MemberApi.FieldAnalyzers;

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
        if (context.DataMemberContext.State == DataMemberContextState.Included)
            return context.Symbol.DeclaredAccessibility.IsHiddenVisibleToEditor(context.DataMemberContext);
        return false;
    }
}