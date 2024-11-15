using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.MemberApi.Data;
using System.Text;

namespace NexYamlSourceGenerator.Templates;
internal class DeserializeEmitter
{
    public string Create(ClassPackage package)
    {
        var info = package.ClassInfo;
        var objectCreation = new StringBuilder();
        var map = MapPropertiesToLength(package.MemberSymbols);

        foreach (var symbol in package.MemberSymbols)
        {
            objectCreation.Append(symbol.Name + "=__TEMP__" + symbol.Name + ",");
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
        {{package.CreateTempMembers()}}
                stream.ReadMapping((key) => {
        {{ifstatement}}

                });

                var __TEMP__RESULT = new {{info.NameDefinition}}
                {
                    {{objectCreation.ToString().Trim(',')}}
                };
                {{CreateReferenceFallback(package.MemberSymbols,package.ClassInfo)}}
             
                 value = __TEMP__RESULT;
        """;
    }

    private string CreateReferenceFallback(IEnumerable<SymbolInfo> properties,ClassInfo classInfo)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(var x in properties)
        {
            if (!x.IsInit)
            {
                stringBuilder.AppendLine($"if(__TEMP__RESULT__{x.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{x.Name}.Reference, (obj) => __TEMP__RESULT.{x.Name} = ({x.Type}{(x.IsArray ? "[]" : "")})obj); }}");
            }
            else
            {
                stringBuilder.AppendLine($"if(__TEMP__RESULT__{x.Name}.IsReference) {{ stream.AddReference(__TEMP__RESULT__{x.Name}.Reference, (obj) => SetInit(\"{x.Name}\",__TEMP__RESULT)); }}");
                
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
        switchBuilder.AppendLine($"\t\t\t\t!stream.TryRead(ref __TEMP__{symbol.Name}, ref key,{"UTF8" + symbol.Name}, ref __TEMP__RESULT__{symbol.Name}) &&");
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
