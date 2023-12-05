using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace NexYamlSourceGenerator.NexAPI
{
    /// <summary>
    /// Represents information about a class and its members.
    /// Usable with Incremental Source Generation.
    /// </summary>
    internal class ClassInfo : IEquatable<ClassInfo>
    {
        private const string GeneratorPrefix = "NexSourceGenerated_";
        /// <summary>
        /// i.e. Test<J,K >
        /// or just Test if its non generic
        /// </summary>
        internal string NameDefinition { get; private set; }
        /// <summary>
        /// i.e. <J,K>
        /// or "" if its non generic
        /// </summary>
        internal string TypeParameterArguments { get; private set; }
        internal ClassInfo() { }
        internal string NameSpace { get; private set; }
        internal string GeneratorName { get; private set; }
        internal string Accessor { get; private set; }
        internal IReadOnlyList<string> AllInterfaces { get; private set; }
        internal IReadOnlyList<string> AllAbstracts { get; private set; }

        /// <summary>
        /// i.e. Test<,,>
        /// or Test if it's non generic
        /// </summary>
        internal string ShortDefinition { get; private set; }

        public static ClassInfo CreateFrom(ITypeSymbol type)
        {
            string displayName = type.ToDisplayString();

            int index = displayName.IndexOf('<');
            string shortDefinition = index != -1 ? displayName.Substring(0, index) : displayName;
            string genericTypeArguments = "";

            TryAddGenericsToName(type, ref shortDefinition, ref genericTypeArguments);

            return new()
            {
                NameDefinition = type.ToDisplayString(),
                ShortDefinition = shortDefinition,
                TypeParameterArguments = genericTypeArguments,
                NameSpace = GetFullNamespace(type, '.'),
                AllInterfaces = type.AllInterfaces.Select(t => t.Name).ToList(),
                AllAbstracts = FindAbstractClasses(type),
                Accessor = type.DeclaredAccessibility.ToString().ToLower(),
                GeneratorName = CreateGeneratorName(type)
            };
        }

        private static string CreateGeneratorName(ITypeSymbol type)
        {
            return GeneratorPrefix + GetFullNamespace(type, '_') + type.Name;
        }
        /// <summary>
        /// Attempts to add generic information to the short definition and type parameter arguments.
        /// </summary>
        /// <param name="type">The <see cref="INamedTypeSymbol"/> representing the class.</param>
        /// <param name="shortDefinition">The short definition of the class name.</param>
        /// <param name="genericTypeArguments">The type parameter arguments for the class.</param>
        private static void TryAddGenericsToName(ITypeSymbol type, ref string shortDefinition, ref string genericTypeArguments)
        {
            if (type is INamedTypeSymbol namedType)
            {
                if (namedType.TypeArguments != null)
                {

                    var genericcount = namedType.TypeArguments.Count();
                    if (genericcount > 0)
                    {
                        shortDefinition += "<" + new string(',', genericcount - 1) + ">";
                        genericTypeArguments += "<";
                        foreach (var argument in namedType.TypeArguments)
                        {
                            genericTypeArguments += argument.Name;
                        }
                        genericTypeArguments += ">";
                    }
                }
            }
        }

        static string GetFullNamespace(ITypeSymbol typeSymbol, char separator)
        {
            INamespaceSymbol namespaceSymbol = typeSymbol.ContainingNamespace;
            string fullNamespace = "";

            while (namespaceSymbol != null && !string.IsNullOrEmpty(namespaceSymbol.Name))
            {
                fullNamespace = namespaceSymbol.Name + separator + fullNamespace;
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }

            return fullNamespace.TrimEnd(separator);
        }
        /// <summary>
        /// Finds abstract classes in the inheritance hierarchy of the specified <see cref="ITypeSymbol"/>.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> for which to find abstract classes.</param>
        /// <returns>A list of abstract classes in the inheritance hierarchy of the specified <see cref="ITypeSymbol"/>.</returns>
        private static IReadOnlyList<string> FindAbstractClasses(ITypeSymbol typeSymbol)
        {
            List<string> result = new List<string>();
            INamedTypeSymbol baseType = typeSymbol.BaseType;
            while (baseType != null)
            {
                if (baseType.IsAbstract)
                {
                    result.Add(baseType.ToDisplayString());
                }
                baseType = baseType.BaseType;
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is ClassInfo info &&
                   NameDefinition == info.NameDefinition &&
                   TypeParameterArguments == info.TypeParameterArguments &&
                   NameSpace == info.NameSpace &&
                   GeneratorName == info.GeneratorName &&
                   Accessor == info.Accessor &&
                   EqualityComparer<IReadOnlyList<string>>.Default.Equals(AllInterfaces, info.AllInterfaces) &&
                   EqualityComparer<IReadOnlyList<string>>.Default.Equals(AllAbstracts, info.AllAbstracts) &&
                   ShortDefinition == info.ShortDefinition;
        }

        public override int GetHashCode()
        {
            int hashCode = -1139297379;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameDefinition);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeParameterArguments);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameSpace);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GeneratorName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Accessor);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyList<string>>.Default.GetHashCode(AllInterfaces);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyList<string>>.Default.GetHashCode(AllAbstracts);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortDefinition);
            return hashCode;
        }

        public bool Equals(ClassInfo info)
        {
            return
               NameDefinition == info.NameDefinition &&
               TypeParameterArguments == info.TypeParameterArguments &&
               NameSpace == info.NameSpace &&
               GeneratorName == info.GeneratorName &&
               Accessor == info.Accessor &&
               EqualityComparer<IReadOnlyList<string>>.Default.Equals(AllInterfaces, info.AllInterfaces) &&
               EqualityComparer<IReadOnlyList<string>>.Default.Equals(AllAbstracts, info.AllAbstracts) &&
               ShortDefinition == info.ShortDefinition;
        }
    }
}