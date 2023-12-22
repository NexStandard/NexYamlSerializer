using NexYamlSourceGenerator.NexAPI;
using System.Text;

namespace NexYamlSourceGenerator.Templates.Registration;
internal class UTF8MemberEmitter : ITemplate
{
    public string Create(ClassPackage package)
    {
        StringBuilder utf8Members = new StringBuilder();
        foreach(var member in package.MemberSymbols)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(member.Name);
            StringBuilder sb = new StringBuilder();
            foreach (byte by in bytes)
            {
                sb.Append(by + ",");
            }
            utf8Members.Append($"private static readonly byte[] UTF8{member.Name} = new byte[]{{ {sb.ToString().Trim(',')} }};\n\t");
        }
        return utf8Members.ToString();
    }
}
