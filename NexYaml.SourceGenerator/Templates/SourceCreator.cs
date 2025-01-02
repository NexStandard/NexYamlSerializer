﻿using NexYaml.SourceGenerator.MemberApi.Data;
using System.Text;

namespace NexYaml.SourceGenerator.Templates;

internal static class SourceCreator
{
    internal static string ConvertToSourceCode(this ClassPackage package)
    {
        var info = package.ClassInfo;
        var tempVariables = new StringBuilder();
        foreach (var member in package.MemberSymbols)
        {
            tempVariables.AppendLine($"var temp_{member.Name} = default({member.Type});");
        }
        var tag = package.ClassInfo.AliasTag?.Length == 0 ?
            $"{info.NameSpace}.{info.TypeName},{info.NameSpace.Split('.')[0]}" :
            $"{package.ClassInfo.AliasTag}";
        return @$"// <auto-generated/>
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

namespace NexYaml;
[System.CodeDom.Compiler.GeneratedCode(""NexVYaml"",""1.0.0.0"")]
public struct {info.GeneratorName + "Helper"} : IYamlSerializerFactory
{{

    public void Register(IYamlSerializerResolver resolver)
    {{
{package.CreateRegisterThis()}
{package.CreateRegisterAbstracts()}
{package.CreateRegisterInterfaces()}
    }}
{package.CreateInstantiateMethodTyped()}
}}
file sealed class ExternWrapper{info.TypeParameterArguments} {info.TypeParameterRestrictions}
{{
{package.CreateExternCalls()}
}}
[System.CodeDom.Compiler.GeneratedCode(""NexVYaml"",""1.0.0.0"")]
file sealed class {info.GeneratorName + info.TypeParameterArguments} : YamlSerializer<{info.NameDefinition}> {info.TypeParameterRestrictions}
{{
    {(info.DataStyle != "DataStyle.Any" ? $"protected override DataStyle Style {{ get; }} = {info.DataStyle};" : "")}
    {package.CreateUTF8Members()}

    public override void Write(IYamlWriter stream, {info.NameDefinition} value, DataStyle style = {info.DataStyle})
    {{
        style = style == DataStyle.Any ? Style : style;
        stream.BeginMapping(style);
        stream.WriteTag(""!{tag}"");
{package.CreateNewSerializationEmit()}
        stream.EndMapping();
    }}

    public override void Read(IYamlReader stream, ref {info.NameDefinition} value, ref ParseResult parseResult)
    {{
{package.CreateDeserialize()}
    }}
}}
";
    }
}