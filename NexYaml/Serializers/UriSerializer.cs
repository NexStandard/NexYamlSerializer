using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Silk.NET.Maths;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : YamlSerializer<Uri>
{
    public static readonly UriSerializer Instance = new();
    public override void Write<X>(WriteContext<X> context, Uri value, DataStyle style)
    {
        context.WriteScalar(value.ToString());
    }

    public override ValueTask<Uri?> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(new Uri(scalarScope.Value, UriKind.RelativeOrAbsolute));
    }
}
