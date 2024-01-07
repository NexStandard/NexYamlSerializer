using NexYamlSourceGenerator.MemberApi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.Templates;
internal static class CreateFromParent
{
    public static string CreateMethod(this ClassPackage package)
    {
        string w = "";
        foreach (var inter in package.ClassInfo.AllInterfaces)
        {
            if(package.ClassInfo.IsGeneric)
            {
                var t = package.ClassInfo.TypeParameters;
                var ins = inter.TypeParameters;
                var indexArray = CreateIndexArray(t, ins);
                w += $$"""
                            if(typeof(T) == typeof({{inter.ShortDisplayString}})) {
                                var generatorType = typeof({{package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort }});
                                var genericParams = typeof(T).GenericTypeArguments;
                                var param = {{indexArray}};
                                var filledGeneratorType = generatorType.MakeGenericType(param);
                                return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
                            }
                    """;
            }
            else
            {

                w += $"if(typeof(T) == typeof({inter.ShortDisplayString})) {{ return new {package.ClassInfo.GeneratorName}(); }}"; 

            }
        }

           var s = $$"""
            public IYamlFormatter Create<T>()
            {
                {{w}}
                return new EmptyFormatter<T>();
            }
        """;
        
        return s;
    }
    public static string CreateMethodTyped(this ClassPackage package)
    {
        StringBuilder w = new();
        foreach (var inter in package.ClassInfo.AllInterfaces)
        {
            w.Append(CreateIfs(package, inter));
        }
        foreach (var inter in package.ClassInfo.AllAbstracts)
        {
            w.Append(CreateIfs(package, inter));
        }
        string s;
        if (package.ClassInfo.IsGeneric)
        {
             s = $$"""
            public IYamlFormatter Create(Type type)
            {
                {{w}}
                 var gen = typeof({{package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}});
                 var genParams = type.GenericTypeArguments;
                 var fillGen = gen.MakeGenericType(genParams);
                 return (IYamlFormatter)Activator.CreateInstance(fillGen);
            }
        """;
        }
        else
        {
            s = $$"""
            public IYamlFormatter Create(Type type)
            {
                {{w}}
                return new {{package.ClassInfo.GeneratorName}}();
            }
        """;
        }

        return s;
    }

    private static StringBuilder CreateIfs(ClassPackage package, DataPackage inter)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (package.ClassInfo.IsGeneric)
        {
            var t = package.ClassInfo.TypeParameters;
            var ins = inter.TypeParameters;
            var indexArray = CreateIndexArray(t, ins);
            var compare = inter.IsGeneric ? "type.GetGenericTypeDefinition()" : "type";
            stringBuilder.AppendLine($$"""
                            if({{compare}} == typeof({{inter.ShortDisplayString}})) 
                            {
                                var generatorType = typeof({{package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}});
                                var genericParams = type.GenericTypeArguments;
                                var param = {{indexArray}};
                                var filledGeneratorType = generatorType.MakeGenericType(param);
                                return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
                            }
                    """);
        }
        else
        {
            stringBuilder.AppendLine($"\t\t\tif(type == typeof({inter.ShortDisplayString})) {{ return new {package.ClassInfo.GeneratorName}(); }}");
        }

        return stringBuilder;
    }

    static string CreateIndexArray(string[] classTypeParameters, string[] parentTypeParameters)
    {
        int[] indexArray = new int[classTypeParameters.Length];

        for (int i = 0; i < classTypeParameters.Length; i++)
        {
            // Find the index of the matching type parameter in parentTypeParameters
            indexArray[i] = Array.IndexOf(parentTypeParameters, classTypeParameters[i]);
        }

        return $"new Type[] {{ {string.Join(", ", indexArray.Select(index => $"genericParams[{index}]"))} }}";
    }
}
