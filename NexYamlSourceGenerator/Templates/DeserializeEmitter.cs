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
        package.MemberSymbols.ForEach(member => objectCreation.Append(member.Name + "=__TEMP__" + member.Name + ","));
        string ifstatement = "\t\t\t\tparser.SkipRead();";

        if(map.Count > 0)
        {
            ifstatement = $$"""
                        if(
            {{MapPropertiesToSwitch(map)}}
                        ) 
                        {
                            parser.SkipRead(); 
                        }
            
            """;
        }
        return $$"""
                parser.ReadWithVerify(ParseEventType.MappingStart);
        {{package.CreateTempMembers()}}
                while (parser.HasMapping(out var key))
                {
        {{ifstatement}}
                }

                parser.ReadWithVerify(ParseEventType.MappingEnd);
                var __TEMP__RESULT = new {{info.NameDefinition}}
                {
                    {{objectCreation.ToString().Trim(',')}}
                };
             
                 value = __TEMP__RESULT;
        """;
    }
    Dictionary<int, List<SymbolInfo>> MapPropertiesToLength(IEnumerable<SymbolInfo> properties)
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

    void AppendMember(SymbolInfo symbol, StringBuilder switchBuilder)
    {
        switchBuilder.AppendLine($"\t\t\t\t!parser.TryDeserialize(ref __TEMP__{symbol.Name}, ref key,{"UTF8" + symbol.Name}) &&");
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
