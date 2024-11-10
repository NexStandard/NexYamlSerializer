using NexVYaml;
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;
using System.Text;

namespace NexYamlSerializer.Serialization.Formatters;
internal class DelegateFormatter<T> : YamlSerializer<T>
    where T : System.Delegate
{
    public override void Read(IYamlReader stream, ref T value)
    {

        stream.ReadSequence(() =>
        {
            if (stream.TryGetScalarAsSpan(out var span))
            {
                var val = StringEncoding.Utf8.GetString(span);
                stream.ReadWithVerify(ParseEventType.Scalar);
            }
        });
        value = null;
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        var invocations = value.GetInvocationList();
        stream.WriteSequence(style, () =>
        {
            foreach (var invocation in invocations)
            {
                var target = invocation.Target;
                if (target is IIdentifiable id)
                {
                    stream.Write(UTF8Encoding.UTF8.GetBytes(("!!del " + id.Id.ToString() + "#" + invocation.Method.Name)));
                }
            }
        });
    }
}
