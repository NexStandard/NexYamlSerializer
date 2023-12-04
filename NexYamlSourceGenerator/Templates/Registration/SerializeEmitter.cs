using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class SerializeEmitter : ITemplate
    {
        public string Create(ClassInfo info)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SymbolInfo member in info.MemberSymbols)
            {
                if (member.IsAbstract || member.IsInterface)
                {
                    sb.AppendLine($$"""
                            IYamlFormatter<{{member.Type}}> {{member.Name}}formatter = context.Resolver.FindCompatibleFormatter(value.{{member.Name}},value.{{member.Name}}.GetType(),out bool is{{member.Name}}Redirected);
                            if({{member.Name}}formatter is not null)
                            {
                                emitter.WriteString("{{member.Name}}", VYaml.Emitter.ScalarStyle.Plain);
                                context.IsRedirected = is{{member.Name}}Redirected;
                                {{member.Name}}formatter.Serialize(ref emitter, value.{{member.Name}},context);
                            }
                    """);
                }
                else
                {
                    sb.AppendLine($$"""
                        emitter.WriteString("{{member.Name}}", VYaml.Emitter.ScalarStyle.Plain);
                        context.Serialize(ref emitter, value.{{member.Name}});
                """);
                }
            }
            return sb.ToString();
        }
    }
}