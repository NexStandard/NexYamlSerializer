using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers
{
    internal class IsByteArray(IMemberSymbolAnalyzer<IPropertySymbol> analyzer) : MemberSymbolAnalyzer<IPropertySymbol>(new IsArray(analyzer))
    {
        public override bool AppliesTo(MemberContext<IPropertySymbol> context)
        {
            return ((IArrayTypeSymbol)context.Symbol.Type).ElementType.SpecialType == SpecialType.System_Byte;
        }
    }
}