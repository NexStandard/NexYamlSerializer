using NexYamlSourceGenerator.MemberApi.Data;
using System.Collections.Immutable;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

internal static class Registration
{
    public static string CreateRegisterAbstracts(this ClassPackage package)
    {
        return RegisterTypes(package.ClassInfo.AllAbstracts,package.ClassInfo.ShortDefinition , package.ClassInfo.TypeParameters);
    }
    private static string RegisterTypes(ImmutableList<DataPackage> datas, string targetTypeShort, string[] classData)
    {
        StringBuilder sb = new();
        foreach (var data in datas)
        {
            if (CreateFromParent.CreateIndexArray(classData,data.TypeParameters) != null)
            {
                sb.AppendLine($"{Constants.SerializerRegistry}.Register(this,typeof({targetTypeShort}),typeof({data.ShortDisplayString}));");
            }
        }
        return sb.ToString();
    }
    public static string CreateRegisterThis(this ClassPackage package)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag($\"{package.ClassInfo.NameSpace}.{package.ClassInfo.TypeName},{package.ClassInfo.NameSpace.Split('.')[0]}\",typeof({package.ClassInfo.ShortDefinition}));");
        sb.AppendLine($"{Constants.SerializerRegistry}.Register(this,typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.ShortDefinition}));");
        if (package.ClassInfo.AliasTag != "")
        {
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag(\"{package.ClassInfo.AliasTag}\",typeof({package.ClassInfo.ShortDefinition}));");
        }
        if (package.ClassInfo.IsGeneric)
        {
            
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericFormatter(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterFormatter(typeof({package.ClassInfo.ShortDefinition}));");
            return sb.ToString();
        }
        sb.Append("\t\tvar formatter = new ").Append(package.ClassInfo.NameSpace).Append('.').Append(package.ClassInfo.GeneratorName).AppendLine("();");
        return sb.Append(Constants.SerializerRegistry).AppendLine(string.Format(Constants.RegisterFormatter, "formatter")).ToString();
    }
    public static string CreateRegisterInterfaces(this ClassPackage package)
    {
        return RegisterTypes(package.ClassInfo.AllInterfaces, package.ClassInfo.ShortDefinition, package.ClassInfo.TypeParameters);
    }
}