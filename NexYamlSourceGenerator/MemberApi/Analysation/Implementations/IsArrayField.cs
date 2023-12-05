using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;

namespace NexYamlSourceGenerator.MemberApi.Analysation.Implementations
{
    internal class IsArrayField(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
    {
        public override bool AppliesTo(MemberContext<IFieldSymbol> context)
        {
            return context.Symbol.Type.TypeKind == TypeKind.Array;
        }
    }
}