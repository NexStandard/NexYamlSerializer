using NexYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization.Formatters;
public class TypeFormatter : YamlSerializer<Type>
{
    public static YamlSerializer Instance { get; set; } = new TypeFormatter();
    public override void Read(IYamlReader parser, [MaybeNull] ref Type value, ref ParseResult parseResult)
    {
        string? type = null;
        parser.Read(ref type, ref parseResult);
        if(type is not null)
        {
            value = NexYamlSerializerRegistry.Instance.GetAliasType(type);
        }
    }

    public override void Write(IYamlWriter stream, Type value, DataStyle style)
    {
        var alias = stream.Resolver.GetTypeAlias(value);
        stream.Write(alias);
    }
}
