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
        foreach (var member in package.MemberSymbols)
        {
            objectCreation.Append(member.Name + "=__TEMP__" + member.Name + ",");
        }
        return $$"""
                if (parser.IsNullScalar())
                {
                    parser.Read();
                    return default;
                }
                parser.ReadWithVerify(ParseEventType.MappingStart);
        {{package.CreateTempMembers()}}
                while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
                {
                    if (parser.CurrentEventType != ParseEventType.Scalar)
                    {
                        throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
                    }
         
                    if (!parser.TryGetScalarAsSpan(out var key))
                    {
                        throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
                    }
         
                    switch (key.Length)
                    {
        {{MapPropertiesToSwitch(map)}}
                    default:
                        parser.Read();
                        parser.SkipCurrentNode();
                        continue;
                     }
                 }

                 parser.ReadWithVerify(ParseEventType.MappingEnd);
                 var __TEMP__RESULT = new {{info.NameDefinition}}
                 {
                     {{objectCreation.ToString().Trim(',')}}
                 };
             
                 return __TEMP__RESULT;
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
    static void AppendSwitchCase(StringBuilder switchFinder, int propertyLength)
    {
        switchFinder.AppendLine("\t\t\tcase " + propertyLength + ":");
    }
    void AppendArray(string start, SymbolInfo symbol, StringBuilder switchBuilder)
    {
        var serializeString = $"context.DeserializeArray<{symbol.Type}>(ref parser);";

        switchBuilder.AppendLine($$"""
                    {{start}} (key.SequenceEqual({{"UTF8" + symbol.Name}}))
                    {
                        parser.Read();
                        __TEMP__{{symbol.Name}} = {{serializeString}}
                    }
        """);
    }
    void AppendMember(string start, SymbolInfo symbol, StringBuilder switchBuilder)
    {
        switchBuilder.AppendLine($$"""
                    {{start}} (key.SequenceEqual({{"UTF8" + symbol.Name}}))
                    {
                        parser.Read();
                        context.DeserializeWithAlias(ref parser, ref __TEMP__{{symbol.Name}});
                    }
        """);
    }
    public StringBuilder MapPropertiesToSwitch(Dictionary<int, List<SymbolInfo>> properties)
    {
        var switchBuilder = new StringBuilder();
        foreach (var prop in properties)
        {
            AppendSwitchCase(switchBuilder, prop.Key);
            var isFirstime = true;
            foreach (var propert in prop.Value)
            {
                string ifelse;
                if (isFirstime)
                    ifelse = "if";
                else
                    ifelse = "else if";
                if (isFirstime)
                {
                    if (propert.IsArray)
                    {
                        AppendArray(ifelse, propert, switchBuilder);
                    }
                    else
                    {
                        AppendMember(ifelse, propert, switchBuilder);
                    }
                    isFirstime = false;
                }
                else
                {
                    if (propert.IsArray)
                    {
                        AppendArray(ifelse, propert, switchBuilder);
                    }
                    else
                    {
                        AppendMember(ifelse, propert, switchBuilder);
                    }
                }
            }
            AppendElseSkip(switchBuilder);
        }
        return switchBuilder;
    }

    void AppendElseSkip(StringBuilder switchBuilder)
    {
        switchBuilder.AppendLine("""
                    else
                    {
                        parser.Read();
                        parser.SkipCurrentNode();
                    }
                    continue;
        """);
    }
}
