using System.Text;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.Templates;

internal static class EmitExtensions
{
    public static string CreateNewSerializationEmit(this ClassPackage package)
    {
        var sb = new StringBuilder();
        foreach (var member in package.MemberSymbols)
        {
            var dataStyle = ", preferedStyle";
            sb.AppendLine($$"""
                        .Write("{{member.Name}}", value.{{member.Name}}{{dataStyle}})
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
        var charMembers = new StringBuilder();

        foreach (var member in package.MemberSymbols)
        {
            var chars = member.Name.ToCharArray();
            var sb = new StringBuilder();

            foreach (var ch in chars)
            {
                sb.Append($"'{ch}', ");
            }

            charMembers
                .AppendLine($"private static readonly char[] UTF8{member.Name} = new char[] {{ {sb.ToString().TrimEnd(',', ' ')} }};")
                .Append("\t");
        }

        return charMembers.ToString();
    }
}