using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.Core;

namespace NexYaml.SourceGenerator.MemberApi.Data;
internal class DataStyleAnalyzer(ISymbol namedType, ReferencePackage package)
{
    public string Analyze(bool defaultValue = false)
    {
        var dataStyle = "DataStyle.Any";
        if (!defaultValue)
        {
            dataStyle = "DataStyle.Any";
        }
        if (namedType.TryGetAttribute(package.DataStyleAttribute, out var dataStyleData) && dataStyleData is { AttributeConstructor.Parameters: [.., { Name: "style" }], ConstructorArguments: [.., { Value: int value }] })
        {
            dataStyle = GetDataStyle(value);
        }
        return dataStyle;
    }
    private static string GetDataStyle(int style)
    {
        return style switch
        {
            // DataStyle.Any
            0 => "DataStyle.Any",
            1 => "DataStyle.Normal",
            2 => "DataStyle.Compact",
            _ => "DataStyle.Any"
        };
    }
}
