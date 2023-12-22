using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
internal class ValidatorMemberMode<T>(
        IMemberSymbolAnalyzer<T> analyzer
        , MemberMode mode
    ) : MemberSymbolAnalyzer<T>(analyzer)
    where T : ISymbol
{
    public override bool AppliesTo(MemberContext<T> symbol)
    {
        return symbol.DataMemberContext.Mode == mode;
    }

}
internal enum MemberMode
{
    Default, Assign, Content
}