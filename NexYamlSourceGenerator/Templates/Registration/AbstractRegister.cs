using NexYamlSourceGenerator.NexAPI;
using System.Text;

namespace NexYamlSourceGenerator.Templates.Registration
{
    internal class AbstractRegister : ITemplate
    {
        public string Create(ClassPackage package)
        {
            StringBuilder sb = new();
            foreach (string @abstract in package.ClassInfo.AllAbstracts)
            {
                sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterAbstractClass, "this", @abstract));
            }
            return sb.ToString();
        }
    }
}