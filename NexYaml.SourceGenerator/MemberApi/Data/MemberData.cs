using System.Text;
using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.Core;

namespace NexYaml.SourceGenerator.MemberApi.Data;

internal record MemberData<T>(T Symbol, DataMemberContext DataMemberContext) where T : ISymbol;

internal record ClassPackage(ClassInfo ClassInfo, EquatableReadOnlyList<SymbolInfo> MemberSymbols)
{
    internal string CreateExternCalls()
    {
        var builder = new StringBuilder();
        foreach (var symbol in MemberSymbols)
        {
            if (symbol.IsInit)
            {
                var s = $$"""
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_{{symbol.Name}}")]
        public extern static void set_{{symbol.Name}}({{ClassInfo.NameDefinition}} target, {{symbol.Type}} value);
    """;
                builder.AppendLine(s);
            }

        }
        
        return builder.ToString();
    }
}

internal record DataPackage(string DisplayString, string ShortDisplayString, bool IsGeneric, string[] TypeParameters);