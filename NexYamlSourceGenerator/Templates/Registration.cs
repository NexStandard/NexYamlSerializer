using NexYamlSourceGenerator.NexAPI;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

internal static class Registration
{
    public static string CreateRegisterAbstracts(this ClassPackage package)
    {
        StringBuilder sb = new();
        foreach (string @abstract in package.ClassInfo.AllAbstracts)
        {
            sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterAbstractClass, "formatter", @abstract));
        }
        return sb.ToString();
    }
    public static string CreateRegisterThis(this ClassPackage package)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag($\"{package.ClassInfo.NameSpace}.{package.ClassInfo.TypeName},{{AssemblyName}}\",typeof({package.ClassInfo.ShortDefinition}));");
        if (package.ClassInfo.IsGeneric)
        {
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericFormatter(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterFormatter(typeof({package.ClassInfo.ShortDefinition}));");
            return sb.ToString();
        }
        sb.AppendLine($"var formatter = new {package.ClassInfo.NameSpace}.{package.ClassInfo.GeneratorName}();"); ;
        return sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterFormatter, "formatter")).ToString();
    }
    public static string CreateRegisterInterfaces(this ClassPackage package)
    {
        StringBuilder sb = new();
        if (package.ClassInfo.IsGeneric)
        {
            foreach (string interfac in package.ClassInfo.AllInterfaces)
            {
               //  sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, $"typeof({package.ClassInfo.ShortDefinition})", interfac));
            }
        }
        else
        {
            foreach (string interfac in package.ClassInfo.AllInterfaces)
            {
                sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, "formatter", interfac));
            }
        }

        return sb.ToString();
    }
}