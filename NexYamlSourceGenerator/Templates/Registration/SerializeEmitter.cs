using NexYamlSourceGenerator.NexAPI;
using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class SerializeEmitter : ITemplate
    {
        public string Create(ClassPackage package)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SymbolInfo member in package.MemberSymbols)
            {
                string serializeString = $$""".Serialize""";
                if (member.IsArray)
                {
                    serializeString = $$""".SerializeArray""";
                }
                if (member.IsAbstract || member.IsInterface)
                {

                    sb.AppendLine($$"""
                            IYamlFormatter<{{member.Type}}> {{member.Name}}formatter = context.Resolver.FindCompatibleFormatter(value.{{member.Name}},value.{{member.Name}}.GetType(),out bool is{{member.Name}}Redirected);
                            if({{member.Name}}formatter is not null)
                            {
                                emitter.WriteString("{{member.Name}}", VYaml.Emitter.ScalarStyle.Plain);
                                context.IsRedirected = is{{member.Name}}Redirected;
                                {{member.Name}}formatter{{serializeString}}(ref emitter, value.{{member.Name}},context);
                            }
                    """);
                }
                else
                {
                    sb.AppendLine($$"""
                        emitter.WriteString("{{member.Name}}", VYaml.Emitter.ScalarStyle.Plain);
                        context{{serializeString}}(ref emitter, value.{{member.Name}});
                """);
                }
            }
            return sb.ToString();
        }
    }
}