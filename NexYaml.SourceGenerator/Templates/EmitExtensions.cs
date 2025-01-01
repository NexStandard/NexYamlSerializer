using NexYaml.SourceGenerator.MemberApi.Data;
using System.Text;

namespace NexYaml.SourceGenerator.Templates;

internal static class EmitExtensions
{
    public static string CreateNewSerializationEmit(this ClassPackage package)
    {
        var sb = new StringBuilder();
        foreach (var member in package.MemberSymbols)
        {
            var dataStyle = member.DataStyle == "DataStyle.Any" ? ", style" : $", {member.DataStyle}";
            sb.AppendLine($$"""
                        stream.Write("{{member.Name}}", value.{{member.Name}}{{dataStyle}});
                """);

        }
        return sb.ToString();
    }
    public static string CreateTempMembers(this ClassPackage package)
    {
        var defaultValues = new StringBuilder();
        foreach (var member in package.MemberSymbols)
        {
            defaultValues.Append("\t\tvar __TEMP__").Append(member.Name).AppendLine($"= default({(member.IsArray ? member.Type + "[]" : member.Type)});");
            defaultValues.Append("\t\tParseResult __TEMP__RESULT__").Append(member.Name).AppendLine($"= new();");
        }
        return defaultValues.ToString();
    }
    public static string CreateUTF8Members(this ClassPackage package)
    {
        var utf8Members = new StringBuilder();
        foreach (var member in package.MemberSymbols)
        {
            var bytes = Encoding.UTF8.GetBytes(member.Name);
            var sb = new StringBuilder();
            foreach (var by in bytes)
            {
                sb.Append(by + ",");
            }
            utf8Members.AppendLine($"private static readonly byte[] UTF8{member.Name} = new byte[]{{ {sb.ToString().Trim(',')} }};").Append("\t");
        }
        return utf8Members.ToString();
    }
    public static string CreateDeserialize(this ClassPackage package)
    {
        return new DeserializeEmitter().Create(package);
    }
}