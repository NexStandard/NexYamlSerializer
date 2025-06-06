﻿using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;
internal class ValidatorMemberMode<T>(
        IMemberSymbolAnalyzer<T> analyzer
        , MemberMode mode
    ) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(MemberData<T> symbol)
    {
        return symbol.DataMemberContext.Mode == mode;
    }
}
internal enum MemberMode
{
    Default = 0, Assign = 1, Content = 2, Never = 4
}