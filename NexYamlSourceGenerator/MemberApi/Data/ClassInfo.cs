using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.NexIncremental;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace NexYamlSourceGenerator.MemberApi.Data;

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
    internal string[] TypeParameters { get; private set; }
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
    /// <summary>
    /// ie. 
    /// DataStyle.Any
    /// DataStyle.Normal,
    /// DataStyle.Compact
    /// of Stride.Core.DataStyleAttribute
    /// </summary>
    internal string DataStyle { get; init; }
    internal ImmutableList<DataPackage> AllInterfaces { get; private set; }

    internal ImmutableList<DataPackage> AllAbstracts { get; private set; }

    /// <summary>
    /// i.e. Test<,,>
    /// or Test if it's non generic
    /// </summary>
    internal string ShortDefinition { get; private set; }
    public static ClassInfo CreateFrom(INamedTypeSymbol namedType, AttributeData datacontract, ReferencePackage package)
    {
        var displayName = namedType.ToDisplayString();
        var index = displayName.IndexOf('<');
        var shortDefinition = index != -1 ? displayName[..index] : displayName;
        var genericTypeArguments = "";
        var genericTypeArgumentsShort = "";
        var typeParameters = new List<string>();
        var isGeneric = TryAddGenericsToName(namedType, ref shortDefinition, ref genericTypeArguments, ref genericTypeArgumentsShort,typeParameters);
        var restrictions = "";
        var aliasTag = "";
        if (datacontract is { AttributeConstructor.Parameters: [{ Name: "aliasName" }, ..], ConstructorArguments: [{ Value: string alias }, ..] })
        {
            aliasTag = alias;
        }
        DataStyleAnalyzer dataStyleAnalyzer = new DataStyleAnalyzer(namedType, package);
        var dataStyle = dataStyleAnalyzer.Analyze();

        if (isGeneric)
            restrictions = namedType.GenericRestrictions();

        return new()
        {
            NameDefinition = namedType.ToDisplayString(),
            TypeName = namedType.Name,
            AliasTag = aliasTag,
            IsGeneric = isGeneric,
            DataStyle = dataStyle,
            ShortDefinition = shortDefinition,
            TypeParameterArguments = genericTypeArguments,
            TypeParameters = typeParameters.ToArray(),
            TypeParameterRestrictions = restrictions,
            TypeParameterArgumentsShort = new ShortGenericDefinition(namedType.TypeArguments.Length).ToString(),
            NameSpace = GetFullNamespace(namedType, '.'),
            TypeKind = namedType.TypeKind,
            AllInterfaces = GetDataPackages(namedType.AllInterfaces),
            AllAbstracts = GetDataPackages(namedType.FindBase(package)),
            GeneratorName = CreateGeneratorName(namedType)
        };
    }
    private static string GetDataStyle(int style) => style switch
    {
        // DataStyle.Any
        0 => "DataStyle.Normal",
        1 => "DataStyle.Normal",
        2 => "DataStyle.Compact"
    };
    private static ImmutableList<DataPackage> GetDataPackages(ImmutableArray<INamedTypeSymbol> types)
    {
        List<DataPackage> result = new();
        
        foreach (var type in types)
        {
            var display = type.ToDisplayString();
            var shortI = display;
            if (type.IsGenericType)
            {
                shortI = display[..display.IndexOf('<')];
                shortI = shortI + "<" + new string(',', type.TypeArguments.Length - 1) + ">";
            }
            var data = new DataPackage(
                type.ToDisplayString(),
                shortI,
                type.IsGenericType,
                type.TypeArguments.Select(x => x.ToDisplayString()).ToArray()
            );
            result.Add(data);
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
    private static bool TryAddGenericsToName(INamedTypeSymbol type, ref string shortDefinition, ref string genericTypeArguments, ref string genericTypeArgumentsShort, List<string> typeParameters)
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
                    typeParameters.Add(argument.Name);
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

}