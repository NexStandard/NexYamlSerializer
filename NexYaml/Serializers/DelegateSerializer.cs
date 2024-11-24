using Irony.Parsing;
using NexYaml.Core;
using NexYaml.Parser;
using SharpFont;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYaml.Serializers;
internal class DelegateSerializer<T> : YamlSerializer<T>
    where T : Delegate
{
    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        stream.ReadWithVerify(ParseEventType.SequenceStart);
        T val = default;
        while (stream.HasSequence)
        {
            if (stream.TryGetScalarAsSpan(out var dele))
            {
                var strin = StringEncoding.Utf8.GetString(dele);
                var parts = strin.Split('#', 2);
                var id = parts[0];
                var methodName = parts[1];
                var delegateType = typeof(T);
                stream.AddReference(Guid.Parse(id), (obj) =>
                {
                    var method = obj.GetType().GetMethod(methodName);
                    Console.WriteLine(method.GetParameters().Length);
                    Console.WriteLine(method.ReturnParameter);
                    var createdDelegate = method.CreateDelegate<T>(obj);
                    Console.WriteLine(createdDelegate);
                    val = (T)Delegate.Combine(val, createdDelegate);
                });
                stream.ReadWithVerify(ParseEventType.Scalar);
            }
        }
        stream.ReadWithVerify(ParseEventType.SequenceEnd);
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
                stream.Write(System.Text.Encoding.UTF8.GetBytes("!!del " + id.Id.ToString() + "#" + invocation.Method.Name));
            }
        }
        stream.EndSequence();
    }
}
