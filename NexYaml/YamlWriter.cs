using NexYaml.Core;
using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;

namespace NexYaml;

public class YamlWriter : IYamlWriter
{
    /// <summary>
    /// Tracks whether the tag has to be written.
    /// Default to True as the first element always has to be written
    /// </summary>
    private bool IsRedirected { get; set; } = true;
    public HashSet<Guid> References { get; private set; } = new();
    public List<IResolvePlugin> Plugins { get; private set; } 
    public IYamlSerializerResolver Resolver { get; init; }

    internal StyleEnforcer enforcer = new();
    private readonly StreamWriter writer;
    internal EmitterStateMachine StateMachine { get; }

    public YamlWriter(StreamWriter writer,IYamlSerializerResolver resolver, List<IResolvePlugin> plugins)
    {
        StateMachine = new EmitterStateMachine(this);
        Plugins = plugins;
        this.writer = writer;
        Resolver = resolver;
    }

    /// <inheritdoc />
    public void BeginMapping(string tag, DataStyle style)
    {
        var context = new BeginContext()
        {
            Emitter = StateMachine.Current,
            Indentation = StateMachine.IndentationManager
        };
        if (IsRedirected)
        {
            context = new BeginContext()
            {
                NeedsTag = IsRedirected,
                Tag = tag,
                Emitter = StateMachine.Current,
                Indentation = StateMachine.IndentationManager
            };
            IsRedirected = false;
        }
        enforcer.Begin(ref style);
        StateMachine.Next = StateMachine.BeginNodeMap(style, false).Begin(context);
    }
    public void Reset()
    {
        IsRedirected = true;
        enforcer = new();
        References = new();
    }

    /// <inheritdoc />
    public void End()
    {
        var current = StateMachine.Current;
        StateMachine.PopState();
        StateMachine.Current = current.End(StateMachine.Current);
        enforcer.End();
    }

    /// <inheritdoc />
    public void BeginSequence(string tag, DataStyle style)
    {
        var context = new BeginContext()
        {
            Emitter = StateMachine.Current,
            Indentation = StateMachine.IndentationManager
        };
        if (IsRedirected)
        {
            context = new BeginContext()
            {
                NeedsTag = IsRedirected,
                Tag = tag,
                Emitter = StateMachine.Current,
                Indentation = StateMachine.IndentationManager
            };
            IsRedirected = false;
        }
        enforcer.Begin(ref style);
        StateMachine.Next = StateMachine.BeginNodeMap(style, true).Begin(context);
    }

    public void WriteRaw(ReadOnlySpan<char> value)
    {
        writer.Write(value);
    }
    public void WriteRaw(string value)
    {
        writer.Write(value);
    }
    public void WriteRaw(char value)
    {
        writer.Write(value);
    }

    /// <inheritdoc />
    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        ReadOnlySpan<char> byteSpan = Encoding.UTF8.GetChars(value.ToArray());
        StateMachine.WriteScalar(byteSpan);
    }

    /// <inheritdoc />
    public void WriteScalar(ReadOnlySpan<char> value)
    {

            StateMachine.WriteScalar(value);
        
    }

    /// <inheritdoc />
    public void WriteString(string? value, DataStyle style)
    {
        if (value is null)
        {
            WriteScalar(YamlCodes.Null0);
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
            StateMachine.WriteScalar("\"" + value + "\"");
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = (StateMachine.CurrentIndentLevel + 1) * StateMachine.CurrentIndentLevel;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            StateMachine.WriteScalar(scalarStringBuilt.ToString());
        }
    }

    /// <inheritdoc />
    public void WriteType<T>(T value, DataStyle style, WriteContext context = default)
    {
        throw new Exception("Dont call this");
    }
}

