using NexYaml.Core;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml;

public class YamlWriter(UTF8Stream stream, IYamlSerializerResolver resolver) : IYamlWriter
{
    /// <summary>
    /// Tracks whether the tag has to be written.
    /// </summary>
    public bool IsRedirected { get; set; } = false;
    public HashSet<Guid> References { get; private set; } = new();
    /// <summary>
    /// Tracks if the first element is written, if not the <see cref="WriteTag(string)"/> has to be always included.
    /// </summary>
    private bool IsFirst { get; set; } = true;
    public IYamlSerializerResolver Resolver { get; init; } = resolver;
    public SyntaxSettings Settings { get; init; } = new();

    private StyleEnforcer enforcer = new();

    /// <inheritdoc />
    public void BeginMapping(DataStyle style)
    {
        enforcer.Begin(ref style);
        stream.EmitterFactory.BeginNodeMap(style, false).Begin();
    }
    public void Reset()
    {
        IsRedirected = false;
        IsFirst = true;
        enforcer = new();
        References = new();
        stream.Reset();
    }

    /// <inheritdoc />
    public void EndMapping()
    {
        if (stream.Current.State is EmitState.BlockMappingKey or EmitState.FlowMappingKey)
        {
            stream.Current.End();
            enforcer.End();
        }
        else
        {
            throw new YamlException($"Invalid mapping end: {stream.Current}");
        }
    }

    /// <inheritdoc />
    public void BeginSequence(DataStyle style)
    {
        enforcer.Begin(ref style);
        stream.EmitterFactory.BeginNodeMap(style, true).Begin();
    }

    /// <inheritdoc />
    public void EndSequence()
    {
        if (stream.Current.State is EmitState.BlockSequenceEntry or EmitState.FlowSequenceEntry)
        {
            stream.Current.End();
            enforcer.End();
        }
        else
        {
            throw new YamlException($"Current state is not sequence: {stream.Current}");
        }
    }

    /// <inheritdoc />
    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        stream.WriteScalar(value);
    }

    /// <inheritdoc />
    public void WriteScalar(ReadOnlySpan<char> value)
    {
        stream.WriteScalar(value);
    }

    /// <inheritdoc />
    public void WriteString(string? value, DataStyle style)
    {
        if (value is null)
        {
            WriteScalar(['!', '!', 'n', 'u', 'l']);
            return;
        }
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if (scalarStyle is ScalarStyle.Plain or ScalarStyle.Any)
        {
            WriteScalar(value);
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
            stream.WriteScalar("\"" + value + "\"");
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = (stream.CurrentIndentLevel + 1) * stream.CurrentIndentLevel;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            stream.WriteScalar(scalarStringBuilt.ToString());
        }
    }

    /// <inheritdoc />
    public void WriteType<T>(T value, DataStyle style)
    {
        foreach (var syntax in Settings.Plugins)
        {
            if (syntax.Write(this, value, style))
            {
                return;
            }
        }
        var type = typeof(T);

        if (type.IsValueType || type.IsSealed)
        {
            Resolver.GetSerializer<T>().Write(this, value, style);
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetSerializer(value!.GetType(), typeof(T));
            if (valueType != type)
            {
                IsRedirected = true;
            }

            // C# forgets the cast of T when invoking Serialize,
            // this way we can call the serialize method with the "real type"
            // that is in the object
            if (style is DataStyle.Any)
            {
                formatt.Write(this, value!);
            }
            else
            {
                formatt.Write(this, value!, style);
            }
        }
        else
        {
            if (style is DataStyle.Any)
            {
                Resolver.GetSerializer<T>().Write(this, value!);
            }
            else
            {
                Resolver.GetSerializer<T>().Write(this, value!, style);
            }
        }
    }
    public void WriteTag(string tag, bool force)
    {
        if (IsRedirected || IsFirst || force)
        {
            var fulTag = tag;
            stream.Tag(ref fulTag);
            IsRedirected = false;
            IsFirst = false;
        }
    }
}
