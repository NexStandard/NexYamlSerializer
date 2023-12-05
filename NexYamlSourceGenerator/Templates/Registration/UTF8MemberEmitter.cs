using StrideSourceGenerator.NexAPI;
using StrideSourceGenerator.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NexYamlSourceGenerator.Templates.Registration;
internal class UTF8MemberEmitter : ITemplate
{
    public string Create(ClassInfo info)
    {
        StringBuilder utf8Members = new StringBuilder();
        foreach(var member in info.MemberSymbols)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(member.Name);
            StringBuilder sb = new StringBuilder();
            foreach (byte by in bytes)
            {
                sb.Append(by + ",");
            }
            utf8Members.AppendLine($"\tprivate static readonly byte[] UTF8{member.Name} = new byte[]{{ {sb.ToString().Trim(',')} }};");
        }
        return utf8Members.ToString();
    }
}
