using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class IsArray(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberData<IFieldSymbol> context)
    {
        return context.Symbol.Type.TypeKind == TypeKind.Array;
    }
}