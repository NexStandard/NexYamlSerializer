using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.FieldAnalyzers;

internal class ReadOnly(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        var symbol = context.Symbol;
        if (!symbol.IsReadOnly)
            return false;
        if (symbol.Type.TypeKind == TypeKind.Struct || symbol.Type.SpecialType == SpecialType.System_String)
            return false;
        return true;
    }
}