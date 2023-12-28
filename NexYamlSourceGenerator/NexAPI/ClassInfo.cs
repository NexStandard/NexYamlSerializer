using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace NexYamlSourceGenerator.NexAPI;

/// <summary>
/// Represents information about a class and its members.
/// Usable with Incremental Source Generation.
/// </summary>
internal record ClassInfo
{
    private const string GeneratorPrefix = "NexSourceGenerated_";
    /// <summary>
    /// i.e. Test<J,K >
    /// or just Test if its non generic
    /// </summary>
    internal string NameDefinition { get; private set; }
    internal string TypeName { get; private set; }
    /// <summary>
    /// i.e. <J,K>
    /// or "" if its non generic
    /// </summary>
    internal string TypeParameterArguments { get; private set; }
    internal string TypeParameterRestrictions { get; private set; }
    internal string TypeParameterArgumentsShort { get; private set; }
    internal ClassInfo() { }
    internal bool IsGeneric { get; private set; }
    internal string NameSpace { get; private set; }
    internal string GeneratorName { get; private set; }
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
        string genericTypeArgumentsShort = "";
        var isGeneric = TryAddGenericsToName(type, ref shortDefinition, ref genericTypeArguments, ref genericTypeArgumentsShort);
        string restrictions = "";
        if(type is INamedTypeSymbol namedType && isGeneric)
        { 
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("where \n");
            foreach (var typeRestriction in namedType.TypeParameters)
            {
                string constraints = string.Join(",", typeRestriction.ConstraintTypes.Select(restriction => restriction.ToDisplayString()));
                stringBuilder.AppendLine($"{typeRestriction.ToDisplayString()} : {constraints}");
            }
            restrictions = stringBuilder.ToString();
        }
        
        return new()
        {
            NameDefinition = type.ToDisplayString(),
            TypeName = type.Name,
            IsGeneric = isGeneric,
            ShortDefinition = shortDefinition,
            TypeParameterArguments = genericTypeArguments,
            TypeParameterRestrictions = restrictions,
            TypeParameterArgumentsShort = genericTypeArgumentsShort,
            NameSpace = GetFullNamespace(type, '.'),
            AllInterfaces = type.AllInterfaces.Select(t => t.ToDisplayString()).ToList(),
            AllAbstracts = FindAbstractClasses(type),
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
    private static bool TryAddGenericsToName(ITypeSymbol type, ref string shortDefinition, ref string genericTypeArguments, ref string genericTypeArgumentsShort)
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
                    genericTypeArgumentsShort += "<";
                    foreach (var argument in namedType.TypeArguments)
                    {
                        genericTypeArguments += argument.Name;
                        genericTypeArgumentsShort += ",";
                    }
                    genericTypeArgumentsShort = genericTypeArgumentsShort.Remove(genericTypeArgumentsShort.Length - 1);
                    genericTypeArguments += ">";
                    genericTypeArgumentsShort += ">";
                    return true;
                }

            }
        }
        return false;
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
}