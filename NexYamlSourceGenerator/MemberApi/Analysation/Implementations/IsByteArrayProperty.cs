using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;

namespace NexYamlSourceGenerator.MemberApi.Analysation.Implementations
{
    internal class IsByteArrayProperty(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(new IsArrayProperty(analyzer))
    {
        public override bool AppliesTo(MemberContext<IPropertySymbol> context)
        {
            return ((IArrayTypeSymbol)context.Symbol.Type).ElementType.SpecialType == SpecialType.System_Byte;
        }
    }
}