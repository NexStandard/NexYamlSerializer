using NexYamlSourceGenerator.NexAPI;
using System.Text;

namespace NexYamlSourceGenerator.Templates.Registration
{
    internal class InterfaceRegister : ITemplate
    {
        public string Create(ClassPackage package)
        {
            StringBuilder sb = new();
            if (package.ClassInfo.IsGeneric)
            {
                foreach (string interfac in package.ClassInfo.AllInterfaces)
                {
                    sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, $"typeof({package.ClassInfo.ShortDefinition})", interfac));
                }
            }
            else
            {
                foreach (string interfac in package.ClassInfo.AllInterfaces)
                {
                    sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, "this", interfac));
                }
            }

            return sb.ToString();
        }
    }
}