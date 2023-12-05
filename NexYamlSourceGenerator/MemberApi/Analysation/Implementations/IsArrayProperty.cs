using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;

namespace NexYamlSourceGenerator.MemberApi.Analysation.Implementations
{
    internal class IsArrayProperty(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(analyzer)
    {
        public override bool AppliesTo(MemberContext<IPropertySymbol> context)
        {
            return context.Symbol.Type.TypeKind == TypeKind.Array;
        }
    }
}