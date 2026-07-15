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
        public static ValueTask<T?> Read<T>(this Element element, T? context = default)
        {
            return element.Data.Read<T>(element.Tag, context);
        }
        public static ValueTask<T?> Read<T>(this Map map, T? context = default)
        {
            return map.Value.Read<T>(map.Tag, context);
        }
        public static ValueTask<T?> Read<T>(this Scope scope, ReadOnlySpan<char> tag, T? context = default)
        {
            if(scope.IsNull)
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
            ReadOnlySpan<char> refs = ['!', '!', 'r', 'e', 'f'];
            if (tag.SequenceEqual(refs))
            {
                if (Guid.TryParse(scope.AsScalar(), out var id))
                {
                    return scope.IdentifiableResolver.AsyncGetRef<T?>(id);
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
                if (tag.Length == 0)
                {
                    var serializerForT = scope.Resolver.GetSerializer<T>();
                    return serializerForT.Read(scope, context)!;
                }
                else
                {
                    alias = scope.Resolver.GetAliasType(tag);
                    var serializer = scope.Resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(scope, context);
                    return Convert<T>(res);
                }
            }
            else if(!(tag.Length == 0) && ( (alias = scope.Resolver.GetAliasType(tag)) != type || (alias.IsGenericType  && alias.GetGenericTypeDefinition() != type.GetGenericTypeDefinition())))
            {
                var serializer = scope.Resolver.GetSerializer(alias, type);

                var res = serializer.ReadUnknown(scope, context);
                return Convert<T>(res);
            }
            else
            {
                return scope.Resolver.GetSerializer<T?>().Read(scope, context);
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
