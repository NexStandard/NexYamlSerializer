using System.Collections;
using NexYaml.Serializers;
using Stride.Core.Extensions;

namespace NexYaml.Parser
{
    /// <summary>
    /// Represents the kind of a YAML scope node.
    /// </summary>
    public enum ScopeKind
    {
        /// <summary>
        /// A scalar value (string, number, boolean, or null).
        /// </summary>
        Scalar,

        /// <summary>
        /// A mapping (key–value pairs, equivalent to a dictionary/object).
        /// </summary>
        Mapping,

        /// <summary>
        /// A sequence (an ordered list of items).
        /// </summary>
        Sequence
    }


    /// <summary>
    /// Base class for all YAML <see cref="ScopeKind"/>.
    /// </summary>
    public abstract class Scope
    {
        /// <summary>
        /// Gets the <see cref="ScopeKind"/> of this <see cref="Scope"/>.
        /// </summary>
        public abstract ScopeKind Kind { get; }

        /// <summary>
        /// Gets the YAML tag associated with this scope, if any.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets or sets the indentation level of this scope in the source document.
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Gets the parsing <see cref="ScopeContext"/> that created this scope.
        /// </summary>
        public ScopeContext Context { get; }

        /// <summary>
        /// Initializes a new <see cref="Scope"/>.
        /// </summary>
        /// <param name="tag">The YAML tag associated with this scope.</param>
        /// <param name="indent">The indentation level of this scope.</param>
        /// <param name="context">The parsing <see cref="ScopeContext"/> that owns this scope.</param>
        protected Scope(string tag, int indent, ScopeContext context)
        {
            Tag = tag;
            Indent = indent;
            Context = context;
        }
        public ValueTask<T?> Read<T>(ParseContext context)
        {
            if (this is ScalarScope scalar && scalar.Value == "!!null")
            {
                return new ValueTask<T?>(default(T));
            }
            if (typeof(T).IsArray)
            {
                var t = typeof(T).GetElementType()!;
                var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
                var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

                var value = Convert<T>(arraySerializer.ReadUnknown(this, context));
                return value;
            }
            Type type = typeof(T);

            if (Tag.SequenceEqual("!!ref"))
            {
                var refScalar = this.As<ScalarScope>();
                if (Guid.TryParse(refScalar.Value, out var id))
                {
                    return Context.IdentifiableResolver.AsyncGetRef<T?>(id);
                }
                else
                {
                    throw new InvalidCastException($"couldnt cast ref {refScalar.Value}");
                }
            }

            ValueTask<T?> result;
            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                YamlSerializer? serializer;
                if (Tag.IsNullOrEmpty())
                {
                    var formatt = Context.Resolver.GetSerializer<T>();
                    result = formatt.Read(this, context);
                }
                else
                {
                    Type alias = Context.Resolver.GetAliasType(Tag);
                    serializer = Context.Resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(this, context);
                    result = Convert<T>(res);
                }
            }
            else
            {
                result = Context.Resolver.GetSerializer<T?>().Read(this, context);
            }
            return result;
        }
        private static async ValueTask<T?> Convert<T>(ValueTask<object?> task)
        {
            return (T?)(await task);
        }
    }
}
