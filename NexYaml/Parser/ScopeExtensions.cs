using System.Globalization;
using NexYaml.Core;
using NexYaml.Serializers;
using Stride.Core.Extensions;

namespace NexYaml.Parser
{
    public static class ScopeExtensions
    {
        public static ValueTask<T?> Read<T>(this Scope scope)
        {
            return scope.Read<T>(default);
        }
        public static ValueTask<T?> Read<T>(this Scope scope,T? context)
        {
            if (scope is ScalarScope scalar && scalar.Value == YamlCodes.Null)
            {
                return new ValueTask<T?>(default(T));
            }
            if (typeof(T).IsArray)
            {
                var t = typeof(T).GetElementType()!;
                var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
                var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

                var value = Convert<T>(arraySerializer.ReadUnknown(scope, context));
                return value;
            }
            Type type = typeof(T);

            if (scope.Tag.SequenceEqual("!!ref"))
            {
                var refScalar = scope.As<ScalarScope>();
                if (Guid.TryParse(refScalar.Value, out var id))
                {
                    return scope.Context.IdentifiableResolver.AsyncGetRef<T?>(id);
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
                if (scope.Tag.IsNullOrEmpty())
                {
                    var formatt = scope.Context.Resolver.GetSerializer<T>();
                    result = formatt.Read(scope, context);
                }
                else
                {
                    Type alias = scope.Context.Resolver.GetAliasType(scope.Tag);
                    serializer = scope.Context.Resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(scope, context);
                    result = Convert<T>(res);
                }
            }
            else
            {
                result = scope.Context.Resolver.GetSerializer<T?>().Read(scope, context);
            }
            return result;
        }
        private static async ValueTask<T?> Convert<T>(ValueTask<object?> task)
        {
            return (T?)(await task);
        }
        public static ValueTask<Guid> Read(this Scope scope, Guid value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(Guid.Parse(scalarScope.Value));
        }
        public static ValueTask<Guid?> Read(this Scope scope, Guid? value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(Guid.Parse(scalarScope.Value));
        }
        public static ValueTask<bool> Read(this Scope scope, bool value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(bool.Parse(scalarScope.Value));
        }
        public static ValueTask<bool?> Read(this Scope scope, bool? value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(bool.Parse(scalarScope.Value));
        }
        public static ValueTask<byte> Read(this Scope scope, byte value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(byte.Parse(scalarScope.Value));
        }
        public static ValueTask<byte?> Read(this Scope scope, byte? value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(byte.Parse(scalarScope.Value));
        }
        public static ValueTask<int> Read(this Scope scope, int value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(int.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
        }
        public static ValueTask<int?> Read(this Scope scope, int? value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(int.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
        }

        public static ValueTask<string?> Read(this Scope scope, string? value = default)
        {
            var scalarScope = scope.As<ScalarScope>();
            if (scalarScope.Value == YamlCodes.Null)
                return default;
            return new(scalarScope.Value);
        }

    }
}
