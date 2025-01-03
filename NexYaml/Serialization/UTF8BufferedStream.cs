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
    Stream stream;
    public UTF8BufferedStream(Stream stream)
    {
        this.stream = stream;
        Reset();
    }

    public override object Clone()
    {
        throw new NotImplementedException();
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
        StateStack.Current.WriteScalar(value);
    }
}
