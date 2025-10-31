using System.Text;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.Templates;
internal static class CreateFromParent
{
    public static string CreateInstantiateMethodTyped(this ClassPackage package)
    {
        StringBuilder w = new();
        foreach (var inter in package.ClassInfo.AllInterfaces)
        {
            w.Append(CreateIfs(package, inter, "IYamlSerializer"));
        }
        foreach (var inter in package.ClassInfo.AllAbstracts)
        {
            w.Append(CreateIfs(package, inter, "IYamlSerializer"));
        }
        string s;
        if (package.ClassInfo.IsGeneric)
        {
            s = $$"""
    public IYamlSerializer Instantiate(Type type)
    {
        var genericTypeDefinition = type.GetGenericTypeDefinition();
{{w}}
        var gen = typeof({{package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}});
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlSerializer)Activator.CreateInstance(fillGen)!;
    }
""";
        }
        else
        {
            s = $$"""
            public IYamlSerializer Instantiate(Type type)
            {
        {{w}}
                return new {{package.ClassInfo.GeneratorName}}();
            }
        """;
        }

        return s;
    }

    private static StringBuilder CreateIfs(ClassPackage package, DataPackage data, string cast)
    {
        var stringBuilder = new StringBuilder();
        if (package.ClassInfo.IsGeneric)
        {
            var t = package.ClassInfo.TypeParameters;
            var ins = data.TypeParameters;
            var indexArray = CreateIndexArray(t, ins);
            if (indexArray != null)
            {
                var compare = data.IsGeneric ? "genericTypeDefinition" : "type";
                stringBuilder.AppendLine($$"""
                        if({{compare}} == typeof({{data.ShortDisplayString}})) 
                        {
                            var generatorType = typeof({{package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}});
                            var genericParams = type.GenericTypeArguments;
                            var param = {{indexArray}};
                         
                            var filledGeneratorType = generatorType.MakeGenericType(param);
                            return ({{cast}})Activator.CreateInstance(filledGeneratorType)!;
                        }
                    
                    """);
            }
        }
        else
        {
            stringBuilder.AppendLine($"\t\tif(type == typeof({data.ShortDisplayString})) {{ return new {package.ClassInfo.GeneratorName}(); }}");
        }

        return stringBuilder;
    }

    public static string CreateIndexArray(string[] classTypeParameters, string[] parentTypeParameters)
    {
        var indexArray = new int[classTypeParameters.Length];

        for (var i = 0; i < classTypeParameters.Length; i++)
        {
            // Find the index of the matching type parameter in parentTypeParameters
            indexArray[i] = Array.IndexOf(parentTypeParameters, classTypeParameters[i]);
        }
        if (indexArray.Contains(-1))
            return null;
        return $"new Type[] {{ {string.Join(", ", indexArray.Select(index => $"genericParams[{index}]"))} }}";
    }
}
