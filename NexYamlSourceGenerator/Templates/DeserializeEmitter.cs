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

        foreach( var symbol in package.MemberSymbols)
        {
            objectCreation.Append(symbol.Name + "=__TEMP__" + symbol.Name + ",");
        }
        var ifstatement = "\t\t\t\tstream.SkipRead();";

        if(map.Count > 0)
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
             
                 value = __TEMP__RESULT;
        """;
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
        switchBuilder.AppendLine($"\t\t\t\t!stream.TryRead(ref __TEMP__{symbol.Name}, ref key,{"UTF8" + symbol.Name}) &&");
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
