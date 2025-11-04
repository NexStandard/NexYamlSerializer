using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.Templates;

internal static class SourceCreator
{
    internal static string ConvertToSourceCode(this ClassPackage package)
    {
        var info = package.ClassInfo;
        var tempVariables = new StringBuilder();
        var isEmpty = package.MemberSymbols.Count == 0;
        foreach (var member in package.MemberSymbols)
        {
            tempVariables.AppendLine($"var temp_{member.Name} = context.DataMemberMode == DataMemberMode.Content ? ({info.NameDefinition})context.Value : default({member.Type});");
        }
        var tag = package.ClassInfo.AliasTag?.Length == 0 ?
            $"{info.NameSpace}.{info.TypeName},{info.NameSpace.Split('.')[0]}" :
            $"{package.ClassInfo.AliasTag}";
        ///////
        ///
        var objectCreation = new StringBuilder();
        var setResults = new StringBuilder();
        var objTempVariables = new StringBuilder();
        var ifStatementNew = new StringBuilder();
        var awaitsNew = new StringBuilder();
        var map = package.MemberSymbols;
        // needs ID at first place to avoid deadlock on awaits for reference resolving

        var charMembers = new StringBuilder();

        foreach (var member in package.MemberSymbols)
        {
            charMembers
                .AppendLine($"var UTF8{member.Name} = \"{member.Name}\";");
        }

        var orderedSymbols = package.MemberSymbols.OrderByDescending(s => s.Name == "Id").ToList();
        ///
        if (info.TypeKind is Microsoft.CodeAnalysis.TypeKind.Struct)
        {

            objTempVariables.AppendLine($"\t\tvar res = new {info.NameDefinition}() {{");
        }
        else
        {
            objTempVariables.AppendLine($"\t\tvar res = context is not null ? context : new {info.NameDefinition}() {{");
        }
            foreach (var member in orderedSymbols)
            {
                if (member.IsRequired)
                {
                    objTempVariables.AppendLine($"{member.Name} = default,");
                }
            }

        objTempVariables.AppendLine("};");
        foreach (var member in orderedSymbols)
        {
            objTempVariables.AppendLine($"\t\tvar var_{member.Name} = default(ValueTask<{(member.IsArray ? member.Type + "[]" : member.Type)}>);");
            if (member.Context.Mode == MemberApi.UniversalAnalyzers.MemberMode.Content)
            {
                ifStatementNew.AppendLine($"\t\t\tif (map.Key == UTF8{member.Name}){{ var_{member.Name} = map.Value.Read(res.{member.Name}); continue; }}");
            }
            else
            {
                ifStatementNew.AppendLine($"\t\t\tif (map.Key == UTF8{member.Name}){{ var_{member.Name} = map.Value.Read(default({(member.IsArray ? member.Type + "[]" : member.Type)})!); continue; }}");
            }
            if (member.Context.Mode is MemberApi.UniversalAnalyzers.MemberMode.Content)
            {
                awaitsNew.AppendLine($"if(context is not null) {{ res.{member.Name} = await var_{member.Name}; }} else {{ await var_{member.Name};}}");
            }
            else if (member.IsInit)
            {
                awaitsNew.AppendLine($"\t\tExternWrapper{info.TypeParameterArguments}.set_{member.Name}(res,await var_{member.Name});");
            }
            else
            {
                awaitsNew.AppendLine($"\t\tres.{member.Name} = await var_{member.Name};");
            }
            if (package.ClassInfo.IsIIdentifiable && member.Name == "Id")
            {
                awaitsNew.AppendLine("\t\tscope.Context.IdentifiableResolver.RegisterIdentifiable(res.Id, res);");
            }

        }
        var s = $$"""
            {{info.Accessibility.ToString().ToLower()}} static class {{info.GeneratorName}}_Extension
            {
                public static async ValueTask<{{info.NameDefinition}}> Read{{info.TypeParameterArguments}}(this Scope scope, {{info.NameDefinition}} context = default)
                {
                    if(scope is ScalarScope scalar && scalar.Value == "!!null")
                        return default;
            {{charMembers}}
            {{objTempVariables}}
                    var mapping = scope.As<MappingScope>();
                    foreach(var map in mapping)
                    {
            {{ifStatementNew}}
                    }
            {{awaitsNew}}
                    return res;
                }
            """;

        var isIdentifiable = info.AllInterfaces.Any((b) => {
            return b.DisplayString.EndsWith("Stride.Core.IIdentifiable");
        });

        if (info.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct || !isIdentifiable)
        {
            s += $$"""
            public static async ValueTask<{{info.NameDefinition}}?> Read{{info.TypeParameterArguments}}(this Scope scope, {{info.NameDefinition}}? context = default)
            {
                if(scope is ScalarScope scalar && scalar.Value == YamlCodes.Null)
                    return default;
        {{charMembers}}
        {{objTempVariables}}
                var mapping = scope.As<MappingScope>();
                foreach(var map in mapping)
                {
        {{ifStatementNew}}
                }
        {{awaitsNew}}
                return res;
            }
        """;
        }
        s += "}";
    ///
    string writeString = isEmpty ? $"       context.WriteEmptyMapping(\"!{tag}\");" :
        $"""
        var preferedStyle = style is DataStyle.Any or DataStyle.Normal ? Style : style;
        context.BeginMapping("!{tag}",preferedStyle)
        {package.CreateNewSerializationEmit()}
                .End(context);
        """;
        return $$"""
// <auto-generated/>
//  This code was generated by Strides YamlSerializer.
//  Do not edit this file.

#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using NexYaml;
using NexYaml.Serialization;
using NexYaml.Parser;
using NexYaml.Core;
using Stride.Core;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NexYaml;
[System.CodeDom.Compiler.GeneratedCode("NexVYaml","1.0.0.0")]
public struct {{info.GeneratorName + "Helper"}} : IYamlSerializerFactory
{

    public void Register(IYamlSerializerResolver resolver)
    {
{{package.CreateRegisterThis()}}
{{package.CreateRegisterAbstracts()}}
{{package.CreateRegisterInterfaces()}}
    }
{{package.CreateInstantiateMethodTyped()}}
}
file sealed class ExternWrapper{{info.TypeParameterArguments}} {{info.TypeParameterRestrictions}}
{
{{package.CreateExternCalls()}}
}
[System.CodeDom.Compiler.GeneratedCode("NexVYaml","1.0.0.0")]
file sealed class {{info.GeneratorName + info.TypeParameterArguments}} : IYamlSerializer<{{info.NameDefinition}}> {{info.TypeParameterRestrictions}}
{
    {{(info.DataStyle != "DataStyle.Any" ? $"private DataStyle Style =>{info.DataStyle};" : "private DataStyle Style => DataStyle.Any;")}}
    {{package.CreateUTF8Members()}}

    public void Write<X>(WriteContext<X> context, {{info.NameDefinition}} value, DataStyle style) where X : Node
    {
        {{writeString}}
    }

    public async ValueTask<{{info.NameDefinition}}> Read(Scope scope, {{info.NameDefinition}} context)
    {

{{objTempVariables}}
        var mapping = scope.As<MappingScope>();
        foreach(var map in mapping)
        {
{{ifStatementNew}}
        }
{{awaitsNew}}
        return res;
    }
}
{{(info.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct || info.IsSealed ? s : "")}}
""";
    }
}