using NexYamlSourceGenerator.NexAPI;
using System.Runtime.Serialization;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

internal static class Registration
{
    public static string CreateRegisterAbstracts(this ClassPackage package)
    {
        StringBuilder sb = new();
        foreach (var @abstract in package.ClassInfo.AllAbstracts)
        {
            sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterAbstractClass, "formatter", @abstract));
        }
        return sb.ToString();
    }
    public static string CreateRegisterThis(this ClassPackage package)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag($\"{package.ClassInfo.NameSpace}.{package.ClassInfo.TypeName},{{AssemblyName}}\",typeof({package.ClassInfo.ShortDefinition}));");
        if(package.ClassInfo.AliasTag != "")
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag(\"{package.ClassInfo.AliasTag}\",typeof({package.ClassInfo.ShortDefinition}));");
        if (package.ClassInfo.IsGeneric)
        {
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericFormatter(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterFormatter(typeof({package.ClassInfo.ShortDefinition}));");
            return sb.ToString();
        }
        sb.AppendLine($"\t\tvar formatter = new {package.ClassInfo.NameSpace}.{package.ClassInfo.GeneratorName}();"); ;
        return sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterFormatter, "formatter")).ToString();
    }
    public static string CreateRegisterInterfaces(this ClassPackage package)
    {
        StringBuilder sb = new();
        string refe = "formatter";
        if(package.ClassInfo.IsGeneric)
        {
            refe = $"typeof({package.ClassInfo.ShortDefinition})";
        }
        foreach (var interfac in package.ClassInfo.AllInterfaces)
        {
            string interfacDisplay = interfac.DisplayString;
            if (interfac.IsGeneric)
            {
                interfacDisplay = interfac.ShortDisplayString;
            }
            sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, refe, interfacDisplay));
        }


        return sb.ToString();
    }
}