﻿using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class ReadOnly(IMemberSymbolAnalyzer<IFieldSymbol> analyzer) : MemberSymbolAnalyzer<IFieldSymbol>(analyzer)
{
    public override bool AppliesTo(MemberContext<IFieldSymbol> context)
    {
        IFieldSymbol symbol = context.Symbol;
        if (!symbol.IsReadOnly)
            return false;
        if (symbol.Type.TypeKind == TypeKind.Struct || symbol.Type.SpecialType == SpecialType.System_String)
            return false;
        return true;
    }
}