using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace NexYamlSourceGenerator.MemberApi;


internal record ShortGenericDefinition(int Count)
{
    public override string ToString() => Count <= 0 ? "" : $"<{new string(',', Count - 1)}>";
}

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
    /// If an alias was set with DataContract this will be set.
    /// </summary>
    internal string AliasTag { get; private set; } = "";
    /// <summary>
    /// i.e. <J,K>
    /// or "" if its non generic
    /// </summary>
    internal string TypeParameterArguments { get; private set; }
    internal string TypeParameterRestrictions { get; private set; }
    internal string TypeParameterArgumentsShort { get; private set; }
    internal ClassInfo() { }
    internal bool IsGeneric { get; private set; }
    internal TypeKind TypeKind { get; private set; }
    /// <summary>
    /// i.e. AssemblyName.Namespace.Namespace.Namespace
    /// </summary>
    internal string NameSpace { get; private set; }
    internal string GeneratorName { get; private set; }
    internal ImmutableList<(string DisplayString, string ShortDisplayString, bool IsGeneric)> AllInterfaces { get; private set; }

    internal ImmutableList<string> AllAbstracts { get; private set; }

    /// <summary>
    /// i.e. Test<,,>
    /// or Test if it's non generic
    /// </summary>
    internal string ShortDefinition { get; private set; }

    public static ClassInfo CreateFrom(INamedTypeSymbol namedType, AttributeData datacontract)
    {
        var displayName = namedType.ToDisplayString();
        var index = displayName.IndexOf('<');
        var shortDefinition = index != -1 ? displayName.Substring(0, index) : displayName;
        var genericTypeArguments = "";
        var genericTypeArgumentsShort = "";
        var isGeneric = TryAddGenericsToName(namedType, ref shortDefinition, ref genericTypeArguments, ref genericTypeArgumentsShort);
        var restrictions = "";
        var aliasTag = "";
        if (datacontract is { AttributeConstructor.Parameters: [{ Name: "aliasName" }, ..], ConstructorArguments: [{ Value: string alias }, ..] })
        {
            aliasTag = alias;
        }
        if (isGeneric)
            restrictions = namedType.GenericRestrictions();

        return new()
        {
            NameDefinition = namedType.ToDisplayString(),
            TypeName = namedType.Name,
            AliasTag = aliasTag,
            IsGeneric = isGeneric,
            ShortDefinition = shortDefinition,
            TypeParameterArguments = genericTypeArguments,
            TypeParameterRestrictions = restrictions,
            TypeParameterArgumentsShort = new ShortGenericDefinition(namedType.TypeArguments.Count()).ToString(),
            NameSpace = GetFullNamespace(namedType, '.'),
            TypeKind = namedType.TypeKind,
            AllInterfaces = GetInterfaces(namedType.AllInterfaces),
            AllAbstracts = ImmutableList.Create(FindAbstractClasses(namedType).ToArray()),
            GeneratorName = CreateGeneratorName(namedType)
        };
    }

    private static ImmutableList<(string DisplayString, string ShortDisplayString, bool IsGeneric)> GetInterfaces(ImmutableArray<INamedTypeSymbol> interfaces)
    {
        List<(string DisplayString, string ShortDisplayString, bool IsGeneric)> result = [];
        foreach (var interf in interfaces)
        {
            var display = interf.ToDisplayString();
            var shortI = "";
            var isGeneric = false;
            if (interf.IsGenericType)
            {
                isGeneric = true;
                shortI = display.Substring(0, display.IndexOf('<'));
                shortI = shortI + "<" + new string(',', interf.TypeArguments.Length - 1) + ">";
            }
            result.Add(new() { DisplayString = interf.ToDisplayString(), ShortDisplayString = shortI, IsGeneric = isGeneric });
        }
        return ImmutableList.Create(result.ToArray());
    }
    private static string CreateGeneratorName(INamedTypeSymbol type)
    {
        return GeneratorPrefix + GetFullNamespace(type, '_') + type.Name;
    }
    /// <summary>
    /// Attempts to add generic information to the short definition and type parameter arguments.
    /// </summary>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> representing the class.</param>
    /// <param name="shortDefinition">The short definition of the class name.</param>
    /// <param name="genericTypeArguments">The type parameter arguments for the class.</param>
    private static bool TryAddGenericsToName(INamedTypeSymbol type, ref string shortDefinition, ref string genericTypeArguments, ref string genericTypeArgumentsShort)
    {
        if (type is INamedTypeSymbol namedType)
        {
            var genericcount = namedType.TypeArguments.Length;
            if (genericcount > 0)
            {
                shortDefinition += "<" + new string(',', genericcount - 1) + ">";
                genericTypeArguments += "<";
                genericTypeArgumentsShort += "<";
                foreach (var argument in namedType.TypeArguments)
                {
                    genericTypeArguments += argument.Name + ",";
                    genericTypeArgumentsShort += ",";
                }
                genericTypeArguments = genericTypeArguments.TrimEnd(',');
                genericTypeArgumentsShort = genericTypeArgumentsShort.Remove(genericTypeArgumentsShort.Length - 1);
                genericTypeArguments += ">";
                genericTypeArgumentsShort += ">";
                return true;
            }
        }
        return false;
    }

    static string GetFullNamespace(INamedTypeSymbol typeSymbol, char separator)
    {
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        var fullNamespace = "";

        while (namespaceSymbol != null && !string.IsNullOrEmpty(namespaceSymbol.Name))
        {
            fullNamespace = namespaceSymbol.Name + separator + fullNamespace;
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        return fullNamespace.TrimEnd(separator);
    }
    /// <summary>
    /// Finds abstract classes in the inheritance hierarchy of the specified <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> for which to find abstract classes.</param>
    /// <returns>A list of abstract classes in the inheritance hierarchy of the specified <see cref="INamedTypeSymbol"/>.</returns>
    private static IReadOnlyList<string> FindAbstractClasses(INamedTypeSymbol typeSymbol)
    {
        var result = new List<string>();
        var baseType = typeSymbol.BaseType;
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