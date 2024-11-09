using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using NexYamlSerializer.Serialization.PrimitiveSerializers;
using Stride.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NexVYaml;

public class YamlWriter : IYamlWriter
{
    bool IsRedirected { get; set; } = false;
    bool IsFirst { get; set; } = true;
    IUTF8Stream stream { get; set; }
    public IYamlFormatterResolver Resolver{ get; init; }
    StyleEnforcer enforcer = new();
    internal YamlWriter(IUTF8Stream stream, IYamlFormatterResolver resolver)
    {
        Resolver = resolver;
        this.stream = stream;
    }
    public void BeginMapping(DataStyle style)
    {
        enforcer.Begin(ref style);
        stream.EmitterFactory.BeginNodeMap(style, false).Begin();
    }

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
    public void WriteMapping(DataStyle style, Action action)
    {
        BeginMapping(style);
        action();
        EndMapping();
    }
    public void BeginSequence(DataStyle style)
    {
        enforcer.Begin(ref style);
        stream.EmitterFactory.BeginNodeMap(style, true).Begin();

    }
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
    public void WriteSequence(DataStyle style, Action action)
    {
        BeginSequence(style);
        action();
        EndSequence();
    }
    public void WriteScalar(ReadOnlySpan<byte> value)
    {
        stream.WriteScalar(value);
    }
    public void WriteScalar(ReadOnlySpan<char> value)
    {
        stream.WriteScalar(value);
    }
    public void WriteString(string? value, DataStyle style)
    {
        if (value is null)
        {
            WriteScalar(['!','!','n','u','l']);
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
            var indentCharCount = (stream.CurrentIndentLevel + 1) * UTF8Stream.IndentWidth;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            stream.WriteScalar(scalarStringBuilt.ToString());
        }
    }
    public void WriteType<T>(T value, DataStyle style)
    {
        if (value is null)
        {
            WriteScalar(YamlCodes.Null0);
            return;
        }
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Write(this, value, style);
            return;
        }
        var type = typeof(T);

        if (type.IsValueType || type.IsSealed)
        {
            var formatter = Resolver.GetFormatter<T>();
            formatter.Write(this, value, style);
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetFormatter(value!.GetType(), typeof(T));
            if (valueType != type)
                IsRedirected = true;

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
                Resolver.GetFormatter<T>().Write(this, value!);
            }
            else
            {
                Resolver.GetFormatter<T>().Write(this, value!, style);
            }
        }
    }
    public void WriteTag(string tag)
    {
        if (IsRedirected || IsFirst)
        {
            var fulTag = tag;
            stream.Tag(ref fulTag);
            IsRedirected = false;
            IsFirst = false;
        }
    }
}
