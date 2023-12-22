using NexYamlSourceGenerator.NexAPI;
using System.Text;

namespace NexYamlSourceGenerator.Templates.Registration;
internal class DeserializeEmitter : ITemplate
{
    ITemplate TempMemberEmitter = new TempMemberEmitter();
    public string Create(ClassPackage package)
    {
        var info = package.ClassInfo;
        var defaultValues = new StringBuilder();
        var objectCreation = new StringBuilder();
        Dictionary<int, List<SymbolInfo>> map = MapPropertiesToLength(package.MemberSymbols);
        foreach (SymbolInfo member in package.MemberSymbols)
        {
            objectCreation.Append(member.Name + "=" + "__TEMP__" + member.Name + ",");
        }
        return $$"""
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }
            parser.ReadWithVerify(ParseEventType.MappingStart);
            {{TempMemberEmitter.Create(package)}}
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
        Dictionary<int, List<SymbolInfo>> map = new();
        foreach (SymbolInfo property in properties)
        {
            int propertyLength = property.Name.Length;
            if (!map.ContainsKey(propertyLength))
            {
                map.Add(propertyLength, new() { property });
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
        switchFinder.Append("case " + propertyLength + ":");
    }
    void AppendArray(string start, SymbolInfo symbol, StringBuilder switchBuilder)
    {
        var serializeString = $$"""context.DeserializeArray<{{symbol.Type}}>(ref parser);""";
        if (symbol.IsByteType)
            serializeString = "context.DeserializeByteArray(ref parser);";

       switchBuilder.Append($$"""
            {{start}} (key.SequenceEqual({{"UTF8" + symbol.Name}}))
            {
                parser.Read();
                __TEMP__{{symbol.Name}} = {{serializeString}}
            }
        """);
    }
    void AppendMember(string start, SymbolInfo symbol,StringBuilder switchBuilder)
    {
        switchBuilder.Append($$"""
            {{start}} (key.SequenceEqual({{"UTF8" + symbol.Name}}))
            {
                parser.Read();
                __TEMP__{{symbol.Name}} = context.DeserializeWithAlias<{{symbol.Type}}>(ref parser);
            }
            """);
    }
    public StringBuilder MapPropertiesToSwitch(Dictionary<int, List<SymbolInfo>> properties)
    {
        StringBuilder switchBuilder = new StringBuilder();
        foreach (KeyValuePair<int, List<SymbolInfo>> prop in properties)
        {
            AppendSwitchCase(switchBuilder, prop.Key);
            bool isFirstime = true;
            foreach (SymbolInfo propert in prop.Value)
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
        switchBuilder.Append("""
                else
                {
                    parser.Read();
                    parser.SkipCurrentNode();
                }
                continue;
                """);
    }
}
