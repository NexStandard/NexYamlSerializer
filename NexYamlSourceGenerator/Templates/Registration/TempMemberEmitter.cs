using NexYamlSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.Templates.Registration;
internal class TempMemberEmitter : ITemplate
{
    public string Create(ClassPackage package)
    {
        StringBuilder defaultValues = new StringBuilder();
        foreach (SymbolInfo member in package.MemberSymbols)
        {
            defaultValues.Append("var __TEMP__").Append(member.Name).Append($"= default({(member.IsArray ? member.Type + "[]" : member.Type)});\n");
        }
        return defaultValues.ToString();
    }
}
