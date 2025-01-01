using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Plugins;
public class IdentifiableDelegate : IIdentifiable
{
    public Guid Id { get; set; }
    public Func<Delegate> Func { get; set; }
}
internal class DelegatePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (typeof(Delegate).IsAssignableFrom(typeof(T)))
        {
            stream.ReadWithVerify(ParseEventType.SequenceStart);
            T val = default;
            List<(Guid reference, string delegateName)> delegates = new();
            while (stream.HasSequence)
            {
                if (stream.TryGetScalarAsSpan(out var dele))
                {
                    var strin = StringEncoding.Utf8.GetString(dele);
                    var parts = strin.Split('#', 2);
                    var id = parts[0];
                    var methodName = parts[1];
                    delegates.Add((reference: Guid.Parse(id), delegateName: methodName));
                }
                stream.ReadWithVerify(ParseEventType.Scalar);
            }
            stream.ReadWithVerify(ParseEventType.SequenceEnd);
            result = new ParseResult();
            result.IsReference = true;
            result.Reference = Guid.NewGuid();
            stream.Identifiables.Add(new IdentifiableDelegate()
            {
                Id = result.Reference,
                Func = () =>
                {
                    Delegate @delegate = null;
                    foreach (var d in delegates)
                    {
                        var x = stream.Identifiables.First((item) => item.Id == d.reference);
                        if (x is not null)
                        {
                            var method = x.GetType().GetMethod(d.delegateName);
                            var createdDelegate = method.CreateDelegate(typeof(T), x);
                            @delegate = Delegate.Combine(@delegate, createdDelegate);
                        }
                        if (@delegate is not null)
                        {
                            @delegate = @delegate;
                        }
                    }
                    return @delegate;
                }
            });
            value = default!;
            return true;
        }

        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is Delegate @delegate)
        {
            stream.BeginSequence(style);
            var invocations = @delegate.GetInvocationList();
            foreach (var invocation in invocations)
            {
                if (invocation.Target is IIdentifiable identifiable)
                {
                    stream.Write($"{identifiable.Id}#{invocation.Method.Name}");
                }
            }
            stream.EndSequence();
            return true;
        }
        return false;
    }
}
