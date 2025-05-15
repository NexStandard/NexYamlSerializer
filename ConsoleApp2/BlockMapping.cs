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
        return new Context<Sequence>(context.Indent, false, context.StyleScope, new FlowSequence(),context.Writer);
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
    public override void End<T>(Context<T> context)
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
}
class FlowMapping : Mapping
{
    public override Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
    {
        Console.Write(tag+ " {");
        return new Context<Mapping>(context.Indent, false, context.StyleScope, this, context.Writer);
    }

    public override Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
    {
        return new Context<Sequence>(context.Indent, false, context.StyleScope, new FlowSequence(), context.Writer);
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

    public override void End<T>(Context<T> context)
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
}
class BlockSequence : Sequence
{
    public override Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
    {
        if (style is DataStyle.Any or DataStyle.Normal)
        {
            if(context.IsRedirected)
            {
                return new SequenceBlockMapping().BeginMapping(context, tag, style);
                // return new BlockMapping().BeginMapping(context, tag, style);
            }
            else
            {
                return new SequenceBlockMapping().BeginMapping(context, tag, style);
            }
        }
        else
        {
            return new FlowMapping().BeginMapping(context, tag, style);
        }
    }

    public override Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
    {
        if(style is DataStyle.Any or DataStyle.Normal)
        {
            Console.Write(tag);
            return new Context<Sequence>(context.Indent+2, false,DataStyle.Normal,this, context.Writer);
        }
        else
        {
            return new FlowSequence().BeginSequence(context, tag, style);
        }
    }

    public override Context<Sequence> Write<T>(T value, Context<Sequence> context)
    {
        Console.WriteLine();
        Console.Write(new string(' ', Math.Max(context.Indent - 2,0))+ "- ");
        if (value is string s)
        {
            new StringSerializer().Write(s, context);
        }
        else if (value is TestS t)
        {
            new TestSerializer().Write(t, context);
        }
        return context;
    }
}
class SequenceBlockMapping : BlockMapping
{
    public override Context<Mapping> Write<T>(string key, T value, Context<Mapping> context)
    {
        Console.Write($"{key} : ");

        if (value is string s)
        {
            new StringSerializer().Write(s, context);
        }
        else if (value is TestS t)
        {
            new TestSerializer().Write(t, context);
        }

        return context with {
            Node = new BlockMapping()
        };
    }
}
class BlockMapping : Mapping
{
    public override Context<Mapping> BeginMapping<T>(Context<T> context, string tag, DataStyle style)
    {
        if (style is DataStyle.Any or DataStyle.Normal)
        {
            Console.Write(tag);
            return new Context<Mapping>(context.Indent+2, false, context.StyleScope, this, context.Writer);
        }
        else
        {
            return new FlowMapping().BeginMapping(context, tag, style);
        }
    }

    public override Context<Sequence> BeginSequence<T>(Context<T> context, string tag, DataStyle style)
    {
        if(style is DataStyle.Any or DataStyle.Normal)
        {
            throw new NotImplementedException();
        }
        else
        {
            return new FlowSequence().BeginSequence(context, tag, style);
        }
    }

    public override Context<Mapping> Write<T>(string key, T value, Context<Mapping> context)
    {
        Console.WriteLine();
        Console.Write(new string(' ',context.Indent));
        Console.Write($"{key} : ");

        if (value is string s)
        {
            new StringSerializer().Write(s, context);
        }
        else if (value is TestS t)
        {
            new TestSerializer().Write(t, context);
        }

        return context;
    }
}