using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Text;

namespace NexYaml.Serialization.Formatters;
internal class DelegateFormatter<T> : YamlSerializer<T>
    where T : Delegate
{
    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {

        stream.ReadSequence(() =>
        {
            if (stream.TryGetScalarAsSpan(out var span))
            {
                var val = StringEncoding.Utf8.GetString(span);
                stream.ReadWithVerify(ParseEventType.Scalar);
            }
        });
        value = default!;
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        var invocations = value.GetInvocationList();
        stream.BeginSequence(style);
        foreach (var invocation in invocations)
        {
            var target = invocation.Target;
            if (target is IIdentifiable id)
            {
                stream.Write(Encoding.UTF8.GetBytes("!!del " + id.Id.ToString() + "#" + invocation.Method.Name));
            }
        }
        stream.EndSequence();
    }
}
