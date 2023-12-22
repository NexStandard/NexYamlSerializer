using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers
{
    internal class IsArray(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
    {
        public override bool AppliesTo(MemberContext<IFieldSymbol> context)
        {
            return context.Symbol.Type.TypeKind == TypeKind.Array;
        }
    }
}