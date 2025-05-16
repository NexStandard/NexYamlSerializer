using NexYaml.Core;
using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serialization;

public abstract class Writer(IYamlSerializerResolver resolver, IEnumerable<IResolvePlugin> plugins)
{
    public IYamlSerializerResolver Resolver { get; } = resolver;
    public HashSet<Guid> References { get; private set; } = new();
    public abstract void Write(ReadOnlySpan<char> text);
    public void WriteType<X, T>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        foreach(var plugin in plugins)
        {
            if (plugin.Write(context, value, context.StyleScope))
            {
                return;
            }
        }
        var type = typeof(T);

        if (type.IsValueType || type.IsSealed)
        {
            resolver.GetSerializer<T>().Write(context, value, style);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = resolver.GetSerializer(value!.GetType(), typeof(T));
            if (valueType != type)
            {
                context = context with
                {
                    IsRedirected = true
                };
            }

            // C# forgets the cast of T when invoking Serialize,
            // this way we can call the serialize method with the "real type"
            // that is in the object
            if (style is DataStyle.Any)
            {
                formatt.Write(context, value!);
                return;
            }
            else
            {
                formatt.Write(context, value!, style);
                return;
            }
        }
        else
        {
            if (style is DataStyle.Any)
            {
                resolver.GetSerializer<T>().Write(context, value!);
            }
            else
            {
                resolver.GetSerializer<T>().Write(context, value!, style);
            }
        }
    }
    public void WriteString<X>(WriteContext<X> context,string? value, DataStyle style)
        where X : Node
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
            return;
        }
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if(scalarStyle is ScalarStyle.Literal && style is DataStyle.Compact)
        {
            scalarStyle = ScalarStyle.DoubleQuoted;
        }
        if (scalarStyle is ScalarStyle.Plain or ScalarStyle.Any)
        {
            context.WriteScalar(value);
            return;
        }
        else if (ScalarStyle.Folded == scalarStyle)
        {
            throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
        }
        else if (ScalarStyle.SingleQuoted == scalarStyle)
        {
            throw new InvalidOperationException("Single Quote is reserved for char");
        }
        else if (ScalarStyle.DoubleQuoted == scalarStyle)
        {
            context.WriteScalar("\"" + value + "\"");
            return;
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = Math.Max(1,(context.Indent + 1) * context.Indent);
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount).ToString();
            if (scalarStringBuilt.EndsWith("\n"))
            {
                scalarStringBuilt = scalarStringBuilt.Substring(0, scalarStringBuilt.Length - 1);
            }
            context.WriteScalar(scalarStringBuilt);
            return;
        }
        // TODO is this reachable?
        throw new YamlException("Couldnt get ScalarStyle");
    }
}
