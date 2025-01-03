using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Serialization;
public class UTF8BufferStream : UTF8Stream
{
    public UTF8BufferStream(ArrayBufferWriter<byte> buffer)
    {
        this.buffer = buffer;
    }
    ArrayBufferWriter<byte> buffer;
    public override void Flush()
    {
    }

    public override void WriteRaw(ReadOnlySpan<byte> value)
    {
        buffer.Write(value);
    }

    public override void WriteRaw(byte value)
    {
        buffer.Write([ value ]);
    }

    public override void WriteScalar(ReadOnlySpan<byte> value)
    {
        StateStack.Current.WriteScalar(value);
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }
}
