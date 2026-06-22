using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser.Scopes;
using NexYaml.Serializers;
using SharpFont.Cache;
using Silk.NET.Maths;
using Stride.Core.Extensions;

namespace NexYaml.Parser
{
    public static class ScopeExtensions
    {
        public static ValueTask<T?> Read<T>(this Scope scope, T? context = default)
        {
            if(scope.IsNull)
            {
                return new ValueTask<T?>(default(T));
            }
            if (scope.Kind is ScopeKind.Scalar && MemoryExtensions.Equals( scope.AsScalar(),YamlCodes.Null.AsSpan(),StringComparison.OrdinalIgnoreCase))
            {
                return new ValueTask<T?>(default(T));
            }

            if (typeof(T).IsArray)
            {
                var t = typeof(T).GetElementType()!;
                var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
                var arraySerializer = (IYamlSerializer)Activator.CreateInstance(arraySerializerType)!;

                var value = Convert<T>(arraySerializer.ReadUnknown(scope, context));
                return value;
            }

            if (scope.Tag.SequenceEqual("!!ref"))
            {
                if (Guid.TryParse(scope.AsScalar(), out var id))
                {
                    return scope.Context.IdentifiableResolver.AsyncGetRef<T?>(id);
                }
                else
                {
                    throw new InvalidCastException($"couldnt parse ref {scope.AsScalar()}");
                }
            }

            var type = typeof(T);
            Type alias;
            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                if (scope.Tag.IsNullOrEmpty())
                {
                    var serializerForT = scope.Context.Resolver.GetSerializer<T>();
                    return serializerForT.Read(scope, context)!;
                }
                else
                {
                    alias = scope.Context.Resolver.GetAliasType(scope.Tag);
                    var serializer = scope.Context.Resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(scope, context);
                    return Convert<T>(res);
                }
            }
            else if(!scope.Tag.IsNullOrEmpty() && ( (alias = scope.Context.Resolver.GetAliasType(scope.Tag)) != type || (alias.IsGenericType  && alias.GetGenericTypeDefinition() != type.GetGenericTypeDefinition())))
            {
                var serializer = scope.Context.Resolver.GetSerializer(alias, type);

                var res = serializer.ReadUnknown(scope, context);
                return Convert<T>(res);
            }
            else
            {
                return scope.Context.Resolver.GetSerializer<T?>().Read(scope, context);
            }
        }

        private static async ValueTask<T?> Convert<T>(ValueTask<object?> task)
        {
            return (T?)(await task);
        }

        // What are all of these for ? value is left unused ... -Eideren
        public static ValueTask<Guid> Read(this Scope scope, Guid value = default)
        {
            if (scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(Guid.Parse(scalar));
        }
        public static ValueTask<Guid?> Read(this Scope scope, Guid? value = default)
        {
            if(scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(Guid.Parse(scalar));
        }
        public static ValueTask<bool> Read(this Scope scope, bool value = default)
        {
            if (scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(bool.Parse(scalar));
        }
        public static ValueTask<bool?> Read(this Scope scope, bool? value = default)
        {
            if (scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(bool.Parse(scalar));
        }
        public static ValueTask<byte> Read(this Scope scope, byte value = default)
        {
            if(scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(byte.Parse(scalar));
        }
        public static ValueTask<byte?> Read(this Scope scope, byte? value = default)
        {
            if(scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(byte.Parse(scalar));
        }
        public static ValueTask<int> Read(this Scope scope, int value = default)
        {
            if (scope.IsNull) return default;
            return new(int.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
        }
        public static ValueTask<int?> Read(this Scope scope, int? value = default)
        {
            if(scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(int.Parse(scalar, CultureInfo.InvariantCulture));
        }

        public static ValueTask<string?> Read(this Scope scope, string? value = default)
        {
            if(scope.IsNull) return default;
            var scalar = scope.AsScalar();
            return new(scalar.ToString());
        }

    }
}
