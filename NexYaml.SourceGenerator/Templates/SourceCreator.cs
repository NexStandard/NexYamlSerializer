﻿using System.Text;
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
        var ifStatement = new StringBuilder();
        var awaits = new StringBuilder();
        var map = package.MemberSymbols;
        // needs ID at first place to avoid deadlock on awaits for reference resolving
        var orderedSymbols = package.MemberSymbols.OrderByDescending(s => s.Name == "Id").ToList();
        ///
        objTempVariables.AppendLine($"\t\tvar res = context.DataMemberMode is DataMemberMode.Content ? ({info.NameDefinition})context.Value :  new {info.NameDefinition}() {{");
        foreach(var member in orderedSymbols)
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
                objTempVariables.AppendLine($"\t\tvar context_{member.Name} = new ParseContext() {{ DataMemberMode = DataMemberMode.Content, Value = res.{member.Name} }};");
            }
            else
            {
                objTempVariables.AppendLine($"\t\tvar context_{member.Name} = new ParseContext();");
            }
            if (member.Context.Mode is MemberApi.UniversalAnalyzers.MemberMode.Content)
            {
                awaits.AppendLine($"await var_{member.Name};");
            }
            else if (member.IsInit)
            {
                awaits.AppendLine($"\t\tExternWrapper{info.TypeParameterArguments}.set_{member.Name}(res,await var_{member.Name});");
            }
            else
            {
                awaits.AppendLine($"\t\tres.{member.Name} = await var_{member.Name};");
            }
            if (package.ClassInfo.IsIIdentifiable && member.Name == "Id")
            {
                awaits.AppendLine("\t\tstream.RegisterIdentifiable(res.Id, res);");
            }
            ifStatement.AppendLine($"\t\t\tif (key.SequenceEqual(UTF8{member.Name})){{stream.Move();var_{member.Name} = stream.Read<{(member.IsArray ? member.Type + "[]" : member.Type)}>(context_{member.Name}); continue; }}");
        }
        ifStatement.AppendLine("\t\t\tstream.SkipRead();");

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
using NexYaml.Parser;
using NexYaml.Serialization;
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
file sealed class {{info.GeneratorName + info.TypeParameterArguments}} : YamlSerializer<{{info.NameDefinition}}> {{info.TypeParameterRestrictions}}
{
    {{(info.DataStyle != "DataStyle.Any" ? $"protected override DataStyle Style =>{info.DataStyle};" : "")}}
    {{package.CreateUTF8Members()}}

    public override void Write<X>(WriteContext<X> context, {{info.NameDefinition}} value, DataStyle style)
    {
        {{writeString}}
    }

    public override async ValueTask<{{info.NameDefinition}}> Read(IYamlReader stream, ParseContext context)
    {

{{objTempVariables}}
        stream.Move(ParseEventType.MappingStart);
        while(stream.HasMapping(out var key,false))
        {
{{ifStatement}}
        }
        stream.Move(ParseEventType.MappingEnd);
{{awaits}}
        return res;
    }
}
""";
    }
}