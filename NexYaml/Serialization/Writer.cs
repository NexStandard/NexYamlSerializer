using NexYaml.Core;
using NexYaml.Plugins;
using Stride.Core;

namespace NexYaml.Serialization;
/// <summary>
/// Provides an abstract base class for writing YAML content. 
/// This class uses a serializer <see cref="IYamlSerializerResolver"/> and a collection of resolve <see cref="IResolvePlugin"/>
/// to handle the redirection of registered types into YAML.
/// </summary>
public abstract class Writer(IYamlSerializerResolver resolver, IEnumerable<IResolvePlugin> plugins)
{
    /// <summary>
    /// Gets the <see cref="IYamlSerializerResolver"/> used to resolve serializers for the registered types.
    /// </summary>
    public IYamlSerializerResolver Resolver { get; } = resolver;

    /// <summary>
    /// Gets the collection of GUID references representing already serialized <see cref="IIdentifiable"/>.
    /// </summary>
    public HashSet<Guid> References { get; } = [];

    /// <summary>
    /// Writes the provided formatted and escaped text to the underlying output.
    /// </summary>
    /// <param name="text">A <see cref="ReadOnlySpan{T}"/> of characters representing the formatted text to write.</param>
    public abstract void Write(ReadOnlySpan<char> text);

    /// <summary>
    /// Redirects a value to the next <see cref="YamlSerializer{T}"/> using the provided <see cref="WriteContext{T}"/> and <see cref="DataStyle"/>.
    /// </summary>
    /// <typeparam name="X">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/> providing the current state.</param>
    /// <param name="value">The value to write. May be <c>null</c>, in which case a <see cref="YamlCodes.Null0"/> is emitted.</param>
    /// <param name="style">The <see cref="DataStyle"/> to use for the output</param>
    public virtual void WriteType<X, T>(WriteContext<X> context, T? value, DataStyle style)
        where X : Node
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
            return;
        }
        foreach (var plugin in plugins)
        {
            if (plugin.Write(context, value, context.StyleScope))
            {
                return;
            }
        }
        var type = typeof(T);

        if ((type.IsValueType || type.IsSealed) && !type.IsGenericType)
        {
            Resolver.GetSerializer<T>().Write(context, value, style);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetSerializer(value!.GetType(), typeof(T));
            if (valueType != type)
            {
                context = context with
                {
                    IsRedirected = true
                };
            }

            // C# forgets the cast of T when invoking to an object,
            // this way we can call the write method with the "real type"
            // that is in the object
            formatt.Write(context, value, style);
        }
        else
        {
            Resolver.GetSerializer<T>().Write(context, value, style);
        }
    }

    /// <summary>
    /// Writes a string value to the YAML output, formatting it according to the appropriate scalar style.
    /// <list type="bullet">
    ///     <item><description><see cref="ScalarStyle.Plain"/> or <see cref="ScalarStyle.Any"/>: The string is written as-is.</description></item>
    ///     <item><description><see cref="ScalarStyle.Folded"/>: Not supported; throws a <see cref="NotSupportedException"/>.</description></item>
    ///     <item><description><see cref="ScalarStyle.SingleQuoted"/>: Reserved for characters; throws an <see cref="InvalidOperationException"/>.</description></item>
    ///     <item><description><see cref="ScalarStyle.DoubleQuoted"/>: The string is double-quoted with newline characters escaped.</description></item>
    ///     <item><description><see cref="ScalarStyle.Literal"/>: The string is formatted with literal scalar style ( YAML chomping ), adjusting indentations and removing trailing newlines if needed.</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="X">The type of the current <see cref="Node"/>.</typeparam>
    /// <param name="context">The context providing state such as current indentation and style.</param>
    /// <param name="value">The string value to write.</param>
    /// <param name="style">The data style that influences formatting (for example, compact or normal).</param>
    public virtual ReadOnlySpan<char> FormatString<X>(WriteContext<X> context, string value, DataStyle style)
        where X : Node
    {
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if(scalarStyle is ScalarStyle.Literal && style is DataStyle.Compact)
        {
            scalarStyle = ScalarStyle.DoubleQuoted;
        }
        switch (scalarStyle)
        {
            case ScalarStyle.Plain or ScalarStyle.Any:
                return value;
            case ScalarStyle.Folded:
                throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
            case ScalarStyle.SingleQuoted:
                throw new InvalidOperationException("Single Quote is reserved for char");
            case ScalarStyle.DoubleQuoted:
                return "\"" + value.Replace("\n", "\\n").Replace("\"", "\\\"" ) + "\"";
            case ScalarStyle.Literal:
                {
                    var indentCharCount = Math.Max(1, (context.Indent + 1) * context.Indent);
                    var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount).ToString();
                    return scalarStringBuilt;
                }
        }
        throw new ArgumentOutOfRangeException();
    }
}
