using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class IsByteArray(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(new IsArray(analyzer))
{
    public override bool AppliesTo(MemberContext<IFieldSymbol> symbol)
    {
        return ((IArrayTypeSymbol)symbol.Symbol.Type).ElementType.SpecialType == SpecialType.System_Byte;
    }
}