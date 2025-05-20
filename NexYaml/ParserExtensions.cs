using NexYaml.Parser;
using System.Diagnostics.CodeAnalysis;

namespace NexYaml;
public static class ParserExtensions
{
    public static bool IsNullable(this YamlParser stream, Type value, [MaybeNullWhen(false)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
    /* 
    public static MappingReader ReadMapping(this IYamlReader stream) => new(stream);

    public static SequenceScopeStruct SequenceScope(this IYamlReader stream) => new SequenceScopeStruct(stream);

    public static SequenceReader<T> ReadAsSequenceOf<T>(this IYamlReader stream) => new(stream);

    public struct SequenceScopeStruct : IDisposable
    {
        private IYamlReader _stream;
        
        public SequenceScopeStruct(IYamlReader stream)
        {
            _stream = stream;
            _stream.Move(ParseEventType.SequenceStart);
        }

        public void Dispose() => _stream.Move(ParseEventType.SequenceEnd);
    }

    public ref struct MappingReader
    {
        private ReadOnlySpan<byte> _current;
        public ReadOnlySpan<byte> Current => _current;
        public IYamlReader Stream { get; }

        public MappingReader(IYamlReader stream)
        {
            Stream = stream;
            Stream.Move(ParseEventType.MappingStart);
        }

        public void Dispose() => Stream.Move(ParseEventType.MappingEnd);

        public bool MoveNext() => Stream.HasMapping(out _current);

        public MappingReader GetEnumerator() => this;
    }

    public ref struct SequenceReader<T>
    {
        private T _current;
        public T Current => _current;
        public IYamlReader Stream { get; }

        public SequenceReader(IYamlReader stream)
        {
            _current = default!;
            Stream = stream;
            Stream.Move(ParseEventType.SequenceStart);
        }

        public void Dispose() => Stream.Move(ParseEventType.SequenceEnd);

        public bool MoveNext()
        {
            if (Stream.HasSequence)
            {
                Stream.Read(ref _current);
                return true;
            }

            return false;
        }

        public SequenceReader<T> GetEnumerator() => this;
    }
    */
}
