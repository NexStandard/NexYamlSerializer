using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
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
            stream.Move(ParseEventType.SequenceStart);
            T val = default;
            List<(Guid reference, string delegateName)> delegates = new();
            while (stream.HasSequence)
            {
                if (stream.TryGetScalarAsString(out var strin))
                {
                    var parts = strin.Split('#', 2);
                    var id = parts[0];
                    var methodName = parts[1];
                    delegates.Add((reference: Guid.Parse(id), delegateName: methodName));
                }
                stream.Move(ParseEventType.Scalar);
            }
            stream.Move(ParseEventType.SequenceEnd);
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

    public bool Read<T>(IYamlReader stream, T value, ParseContext<T> result)
    {
        if (typeof(Delegate).IsAssignableFrom(typeof(T)))
        {
            stream.Move(ParseEventType.SequenceStart);
            T val = default;
            List<(Guid reference, string delegateName)> delegates = new();
            while (stream.HasSequence)
            {
                if (stream.TryGetScalarAsString(out var strin))
                {
                    var parts = strin.Split('#', 2);
                    var id = parts[0];
                    var methodName = parts[1];
                    delegates.Add((reference: Guid.Parse(id), delegateName: methodName));
                }
                stream.Move(ParseEventType.Scalar);
            }
            stream.Move(ParseEventType.SequenceEnd);
            // result = new ParseResult();
            // result.IsReference = true;
            // result.Reference = Guid.NewGuid();
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

    public bool Write<T, X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        if (value is Delegate @delegate)
        {

            var invocations = @delegate.GetInvocationList();
            if(invocations.Length is 0)
            {
                context.WriteEmptySequence("!!del");
                return true;
            }
            var newContext = context.BeginSequence("!!del", style);
            foreach (var invocation in invocations)
            {
                if (invocation.Target is IIdentifiable identifiable)
                {
                    newContext = newContext.Write($"{identifiable.Id}#{invocation.Method.Name}");
                }
            }
            newContext.End(context);
            return true;
        }
        return false;
    }
}
