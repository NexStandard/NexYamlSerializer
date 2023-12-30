using NexYamlSourceGenerator.NexAPI;
using System.Linq.Expressions;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

internal static class EmitExtensions
{
    public static string CreateSerializationEmit(this ClassPackage package)
    {
        StringBuilder sb = new StringBuilder();
        foreach (SymbolInfo member in package.MemberSymbols)
        {
            string serializeString = $$""".Serialize""";
            if (member.IsArray)
                serializeString = $$""".SerializeArray""";
            if (member.IsAbstract || member.IsInterface)
            {

                sb.AppendLine($$"""
                        IYamlFormatter<{{member.Type}}> {{member.Name}}formatter = context.Resolver.FindCompatibleFormatter(value.{{member.Name}},value.{{member.Name}}.GetType(),out bool is{{member.Name}}Redirected);
                            if({{member.Name}}formatter is not null)
                            {
                                emitter.WriteString("{{member.Name}}", NexVYaml.Emitter.ScalarStyle.Plain);
                                context.IsRedirected = is{{member.Name}}Redirected;
                                {{member.Name}}formatter{{serializeString}}(ref emitter, value.{{member.Name}},context);
                            }
                    """);
            }
            else
            {
                sb.AppendLine($$"""
                        emitter.WriteString("{{member.Name}}", NexVYaml.Emitter.ScalarStyle.Plain);
                        context{{serializeString}}(ref emitter, value.{{member.Name}});
                """);
            }
        }
        return sb.ToString();
    }
    public static string BeginMappingStyle(this ClassPackage package) => package.ClassInfo.Kind == Microsoft.CodeAnalysis.TypeKind.Struct ? "BeginSequence(SequenceStyle.Flow)" : "BeginMapping(MappingStyle.Block)";
    public static string EndMappingStyle(this ClassPackage package) => package.ClassInfo.Kind == Microsoft.CodeAnalysis.TypeKind.Struct ? "EndSequence(false)" : "EndMapping()";
    public static string CreateTempMembers(this ClassPackage package)
    {
        StringBuilder defaultValues = new StringBuilder();
        foreach (SymbolInfo member in package.MemberSymbols)
        {
            defaultValues.Append("\t\tvar __TEMP__").Append(member.Name).Append($"= default({(member.IsArray ? member.Type + "[]" : member.Type)});\n");
        }
        return defaultValues.ToString();
    }
    public static string CreateUTF8Members(this  ClassPackage package)
    {
        StringBuilder utf8Members = new StringBuilder();
        foreach (var member in package.MemberSymbols)
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
    public static string CreateDeserialize(this ClassPackage package)
    {
        return new DeserializeEmitter().Create(package);
    }
}