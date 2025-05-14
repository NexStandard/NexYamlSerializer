using NexYaml.Serialization;
using Stride.Core;
using Stride.Input;

namespace ConsoleApp2;

class FlowSequence : Sequence
{
    public override Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
    {
        return new FlowMapping().BeginMapping(context, tag, style);
    }

    public override Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
    {
        Console.Write(tag+ " [");
        return new Context<Sequence>()
        {
            Indent = context.Indent,
            Node = new FlowSequence(),
            Stream = context.Stream,
            StyleScope = DataStyle.Compact,
        };
    }

    public override Context<Sequence> Write<T>(T value, Context<Sequence> context)
    {
        if (value is string s)
        {
            new StringSerializer().Write(s, context);
        }
        else if (value is TestS t)
        {
            new TestSerializer().Write(t, context);
        }
        return context with
        {
            Node = new FlowSequenceSecondary(),
        };
    }

    public override Node WriteScalar<T>(ReadOnlySpan<char> text, in Context<T> context)
    {
        Console.Write(text.ToString());
        return context.Node;
    }
    public override void End<T>(in Context<T> context)
    {
        Console.Write("]");
    }
}
class FlowSequenceSecondary : FlowSequence
{
    public override Context<Sequence> Write<T>(T value, Context<Sequence> context)
    {
        Console.Write("; ");
        return base.Write(value, context);
    }
    public override Node WriteScalar<T>(ReadOnlySpan<char> text, in Context<T> context)
    {
        Console.Write("; ");
        return base.WriteScalar(text, context);
    }
}
class FlowMapping : Mapping
{
    public override Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
    {
        Console.Write(tag+ " {");
        return new Context<Mapping>()
        {
            Indent = context.Indent,
            Node = this,
            Stream = context.Stream,
            StyleScope = DataStyle.Compact,
        };
    }

    public override Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
    {
        return new Context<Sequence>()
        {
            Node = new FlowSequence()
        };
    }

    public override Context<Mapping> Write<T>(string key, T value, Context<Mapping> context)
    {
        Console.Write(key + ": ");
        if(value is string s)
        {
            new StringSerializer().Write(s, context);
        }
        else if( value is TestS t)
        {
            new TestSerializer().Write(t, context);
        }
        return context with
        {
            Node = new FlowMappingSecondary()
        };
    }

    public override Node WriteScalar<T>(ReadOnlySpan<char> text, in Context<T> context)
    {
        Console.Write(text.ToString());
        return new FlowMappingSecondary();

    }
    public override void End<T>(in Context<T> context)
    {
        Console.Write("}");
    }
}
class FlowMappingSecondary : FlowMapping
{
    public override Context<Mapping> Write<T>(string key, T value, Context<Mapping> context)
    {
        Console.Write(", ");
        return base.Write(key, value, context);
    }
    public override Node WriteScalar<T>(ReadOnlySpan<char> text, in Context<T> context)
    {
        return base.WriteScalar(text, context);
    }
}