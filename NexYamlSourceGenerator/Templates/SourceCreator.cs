﻿using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

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
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

using NexYamlSerializer.Serialization.Formatters;

namespace NexVYaml;
[System.CodeDom.Compiler.GeneratedCode(""NexVYaml"",""1.0.0.0"")]
public struct {info.GeneratorName + "Helper" } : IYamlFormatterHelper
{{
    public void Register(IYamlFormatterResolver resolver)
    {{
{package.CreateRegisterThis()}
{package.CreateRegisterAbstracts()}
{package.CreateRegisterInterfaces()}
    }}
{package.CreateInstantiateMethodTyped()}
}}
[System.CodeDom.Compiler.GeneratedCode(""NexVYaml"",""1.0.0.0"")]
file sealed class {info.GeneratorName + info.TypeParameterArguments} : YamlSerializer<{info.NameDefinition}> {info.TypeParameterRestrictions}
{{
    { (info.DataStyle != "DataStyle.Any" ? $"protected override DataStyle Style {{ get; }} = {info.DataStyle};" : "")}
    {package.CreateUTF8Members()}

    protected override void Write(IYamlWriter stream, {info.NameDefinition} value, DataStyle style = {info.DataStyle})
    {{
        stream.{package.BeginMappingStyle()};
        stream.WriteTag(""{tag}"");
{package.CreateNewSerializationEmit()}
        stream.EndMapping();
    }}

    protected override void Read(IYamlReader stream, ref {info.NameDefinition} value)
    {{
{package.CreateDeserialize()}
    }}
}}
";
    }
}