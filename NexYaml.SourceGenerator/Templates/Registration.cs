using NexYaml.SourceGenerator.MemberApi.Data;
using System.Collections.Immutable;
using System.Text;

namespace NexYaml.SourceGenerator.Templates;

internal static class Registration
{
    public static string CreateRegisterAbstracts(this ClassPackage package)
    {
        return RegisterTypes(package.ClassInfo.AllAbstracts, package.ClassInfo.ShortDefinition, package.ClassInfo.TypeParameters);
    }
    private static string RegisterTypes(ImmutableList<DataPackage> datas, string targetTypeShort, string[] classData)
    {
        StringBuilder sb = new();
        foreach (var data in datas)
        {
            if (CreateFromParent.CreateIndexArray(classData, data.TypeParameters) != null)
            {
                sb.AppendLine($"{Constants.SerializerRegistry}.Register(this,typeof({targetTypeShort}),typeof({data.ShortDisplayString}));");
            }
        }
        return sb.ToString();
    }
    public static string CreateRegisterThis(this ClassPackage package)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Constants.SerializerRegistry}.Register(this,typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.ShortDefinition}));");
        if (package.ClassInfo.AliasTag != "")
        {
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag(\"{package.ClassInfo.AliasTag}\",typeof({package.ClassInfo.ShortDefinition}));");
        }
        else
        {
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag($\"{package.ClassInfo.NameSpace}.{package.ClassInfo.TypeName},{package.ClassInfo.NameSpace.Split('.')[0]}\",typeof({package.ClassInfo.ShortDefinition}));");
        }
        if (package.ClassInfo.IsGeneric)
        {

            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericSerializer(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterSerializer(typeof({package.ClassInfo.ShortDefinition}));");
            return sb.ToString();
        }
        sb.Append("\t\tvar serializer = new ").Append(package.ClassInfo.GeneratorName).AppendLine("();");
        return sb.Append(Constants.SerializerRegistry).AppendLine(string.Format(Constants.RegisterSerializer, "serializer")).ToString();
    }
    public static string CreateRegisterInterfaces(this ClassPackage package)
    {
        return RegisterTypes(package.ClassInfo.AllInterfaces, package.ClassInfo.ShortDefinition, package.ClassInfo.TypeParameters);
    }
}