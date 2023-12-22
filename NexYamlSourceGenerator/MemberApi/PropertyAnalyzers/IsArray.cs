using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers
{
    internal class IsArray(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
    {
        public override bool AppliesTo(MemberContext<IPropertySymbol> context)
        {
            return context.Symbol.Type.TypeKind == TypeKind.Array;
        }
    }
}