using NexYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serializers;
public class TypeSerializer : YamlSerializer<Type>
{
    public static YamlSerializer Instance { get; set; } = new TypeSerializer();
    public override void Read(IYamlReader stream, [MaybeNull] ref Type value, ref ParseResult parseResult)
    {
        string? type = null;
        stream.Read(ref type, ref parseResult);
        if (type is not null)
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
