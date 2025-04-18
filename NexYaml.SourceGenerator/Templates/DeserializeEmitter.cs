using NexYaml.SourceGenerator.MemberApi;
using NexYaml.SourceGenerator.MemberApi.Data;
using NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;
using System.Text;

namespace NexYaml.SourceGenerator.Templates;
internal class DeserializeEmitter
{
    public string Create(ClassPackage package)
    {
        var info = package.ClassInfo;
        var objectCreation = new StringBuilder();
        var setResults = new StringBuilder();
        var objTempVariables = new StringBuilder();
        var map = MapPropertiesToLength(package.MemberSymbols);
        objTempVariables.AppendLine($"var res =  parseResult.DataMemberMode == DataMemberMode.Content ? value : new {info.NameDefinition}();");

        foreach (var symbol in package.MemberSymbols)
        {
            if(symbol.Context.Mode == MemberMode.Content)
            {
                objTempVariables.Append("\t\tvar __TEMP__RESULT__").Append(symbol.Name).AppendLine($"= new ParseResult() {{ DataMemberMode = DataMemberMode.Content }};");
            }
            else
            {
                objTempVariables.Append("\t\tvar __TEMP__RESULT__").Append(symbol.Name).AppendLine($"= new ParseResult();");
            }
            if (symbol.Context.Mode == MemberMode.Content || symbol.IsReadonly)
            {
                objTempVariables.AppendLine($"var __TEMP__{symbol.Name} = res.{symbol.Name};");
            }
            else if (symbol.Context.Mode == MemberMode.Assign)
            {
                objTempVariables.AppendLine($"var __TEMP__{symbol.Name} = default({(symbol.IsArray ? symbol.Type + "[]" : symbol.Type)});");

                if (symbol.IsInit)
                {
                    setResults.AppendLine("#if NET9_0");
                    setResults.AppendLine($"if(__TEMP__RESULT__{symbol.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{symbol.Name}.Reference, (obj) => ExternWrapper{package.ClassInfo.TypeParameterArguments}.set_{symbol.Name}(res,({symbol.Type})obj)); }}");
                    setResults.AppendLine($"else {{ ExternWrapper.set_{symbol.Name}(res,__TEMP__{symbol.Name}); }}");
                    setResults.AppendLine("#endif");
                }
                else
                {
                    setResults.AppendLine($"if(__TEMP__RESULT__{symbol.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{symbol.Name}.Reference, (obj) => res.{symbol.Name} = ({symbol.Type}{(symbol.IsArray ? "[]" : "")})obj); }}");
                    setResults.AppendLine($"else {{ res.{symbol.Name} = __TEMP__{symbol.Name}; }}");
                }
            }
        }
        var ifstatement = "\t\t\t\tstream.SkipRead();";

        if (map.Count > 0)
        {
            ifstatement = $$"""
                        if(
            {{MapPropertiesToSwitch(map)}}
                        ) 
                        {
                            stream.SkipRead(); 
                        }
            
            """;
        }
        return $$"""
        {{objTempVariables}}
                foreach (var key in stream.ReadMapping())
                {
        {{ifstatement}}

                }
                {{setResults}}
                 value = res;
        """;
    }

    private string CreateReferenceFallback(IEnumerable<SymbolInfo> properties, ClassInfo classInfo)
    {
        var stringBuilder = new StringBuilder();
        foreach (var x in properties)
        {
            if (!x.IsInit)
            {
                stringBuilder.AppendLine($"if(__TEMP__RESULT__{x.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{x.Name}.Reference, (obj) => __TEMP__RESULT.{x.Name} = ({x.Type}{(x.IsArray ? "[]" : "")})obj); }}");
            }
            else
            {
                stringBuilder.AppendLine("#if NET9_0");
                stringBuilder.AppendLine($"if(__TEMP__RESULT__{x.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{x.Name}.Reference, (obj) => ExternWrapper{classInfo.TypeParameterArguments}.set_{x.Name}(__TEMP__RESULT,({x.Type})obj)); }}");
                stringBuilder.AppendLine("#endif");
            }
        }
        return stringBuilder.ToString();
    }

    private Dictionary<int, List<SymbolInfo>> MapPropertiesToLength(IEnumerable<SymbolInfo> properties)
    {
        Dictionary<int, List<SymbolInfo>> map = [];
        foreach (var property in properties)
        {
            var propertyLength = property.Name.Length;
            if (!map.ContainsKey(propertyLength))
            {
                map.Add(propertyLength, [property]);
            }
            else
            {
                map[propertyLength].Add(property);
            }
        }
        return map;
    }

    private void AppendMember(SymbolInfo symbol, StringBuilder switchBuilder)
    {
        switchBuilder.AppendLine($"\t\t\t\t!stream.TryRead(ref __TEMP__{symbol.Name}, in key, {"UTF8" + symbol.Name}, ref __TEMP__RESULT__{symbol.Name}) &&");
    }
    public string MapPropertiesToSwitch(Dictionary<int, List<SymbolInfo>> properties)
    {
        var switchBuilder = new StringBuilder();
        foreach (var prop in properties)
        {
            foreach (var propert in prop.Value)
            {
                AppendMember(propert, switchBuilder);
            }
        }
        var result = switchBuilder.ToString();
        result = result.TrimEnd('\r', '\n');
        result = result.TrimEnd('&');
        return result;
    }
}
