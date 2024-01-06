using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class ReadOnly(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(Data<IFieldSymbol> context)
    {
        var symbol = context.Symbol;
        if (!symbol.IsReadOnly)
            return false;
        if (symbol.Type.TypeKind == TypeKind.Struct || symbol.Type.SpecialType == SpecialType.System_String)
            return false;
        return true;
    }
}