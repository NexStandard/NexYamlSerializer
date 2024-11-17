using NexYaml.Parser;
using Silk.NET.Maths;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class DelegatePlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        Type type= typeof(T);
        Console.WriteLine(type.FullName);
        if (value is Delegate @delegate)
        {
            parser.ReadWithVerify(ParseEventType.SequenceStart);
            while (parser.HasSequence)
            {
                if(parser.TryGetCurrentTag(out var tag))
                {
                    if(tag.Handle == "del")
                    {
                        if(parser.TryGetScalarAsString(out var del))
                        {
                            string[] parts = del.Split('#', 2);
                            string id = parts[0];
                            string methodName = parts[1];
                            var delegateType = @delegate.GetType();
                            parser.AddReference(Guid.Parse(id), (obj) => {
                                var method = obj.GetType().GetMethod(methodName);    
                                if(@delegate is null)
                                {
                                    @delegate = Delegate.CreateDelegate(delegateType,method);
                                }
                                else
                                {
                                    @delegate = Delegate.Combine(@delegate, Delegate.CreateDelegate(delegateType, method));
                                }
                            });
                            result.IsReference = true;
                            result.Reference = Guid.Parse(id);
                        }
                    }
                }
            }
            parser.ReadWithVerify(ParseEventType.SequenceEnd);
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
                    stream.WriteScalar($"!!del {identifiable.Id}#{invocation.Method.Name}".AsSpan());
                }
            }
            stream.EndSequence();
            return true;
        }
        return false;
    }
}
