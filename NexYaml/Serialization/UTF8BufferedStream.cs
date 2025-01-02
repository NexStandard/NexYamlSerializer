using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
public class UTF8BufferedStream : UTF8Stream
{

    BufferedStream stream;
    public UTF8BufferedStream(Stream stream)
    {
        this.stream = new BufferedStream(stream);
        Reset();
    }
    public override void Dispose()
    {
        base.Dispose();
        stream.Dispose();
    }
    public override void Flush()
    {
        stream.Flush();
    }

    public override void WriteRaw(ReadOnlySpan<byte> value)
    {
        stream.Write(value);
    }

    public override void WriteRaw(byte value)
    {
        stream.Write([ value ]);
    }

    public override void WriteScalar(ReadOnlySpan<byte> value)
    {
        Span<char> span = stackalloc char[Encoding.UTF8.GetCharCount(value)];

        Encoding.UTF8.GetChars(value, span);
        StateStack.Current.WriteScalar(span);
    }
}
