using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi.Data;
internal class DataStyleAnalyzer(ISymbol namedType, ReferencePackage package)
{
    public string Analyze(bool defaultValue = false)
    {
        string dataStyle = "DataStyle.Normal";
        if (!defaultValue)
        {
            dataStyle = "";
        }
        if (namedType.TryGetAttribute(package.DataStyleAttribute, out var dataStyleData))
        {
            if (dataStyleData is { AttributeConstructor.Parameters: [.., { Name: "style" }], ConstructorArguments: [.., { Value: int value }] })
            {
                dataStyle = GetDataStyle(value);
            }
        }
        return dataStyle;
    }
    private static string GetDataStyle(int style) => style switch
    {
        // DataStyle.Any
        0 => "DataStyle.Normal",
        1 => "DataStyle.Normal",
        2 => "DataStyle.Compact"
    };
}
