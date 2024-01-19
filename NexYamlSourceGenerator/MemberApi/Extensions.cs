using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;
using NexYamlSourceGenerator.MemberApi.FieldAnalyzers;
using NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
using System.Collections.Immutable;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi;
internal static class Extensionss
{
    /// <summary>
    /// Removes all <see cref="SymbolInfo.Empty"/> from the collection.
    /// </summary>
    /// <param name="infos">The collection to filter</param>
    /// <returns>A collection without Empty elements</returns>
    internal static IEnumerable<SymbolInfo> Reduce(this IEnumerable<SymbolInfo> infos)
    {
        foreach (var info in infos)
        {
            if(info != SymbolInfo.Empty)
                yield return info;
        }
    }
    /// <summary>
    /// Tries to get <paramref name="attribute"/> on the <paramref name="symbol"/>
    /// </summary>
    /// <param name="symbol">The Symbol to search on</param>
    /// <param name="attribute">The attribute looking for</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> of the Attribute if it is found</param>
    /// <returns>true if the attribute is found, else false</returns>
    public static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol attribute, out AttributeData attributeData)
    {
        attributeData = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.OriginalDefinition.Equals(attribute, SymbolEqualityComparer.Default) ?? false);
        return attributeData != null;
    }
    // TODO : dont double loop over base classes with find abstracts
    /// <summary>
    /// Retrieves all <see cref="IPropertySymbol"/> and <see cref="IFieldSymbol"/> from the specified <see cref="INamedTypeSymbol"/> and it's base types,
    /// returning them in reverse order of inheritance ( top to bottom ).
    /// </summary>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to retrieve members for.</param>
    /// <param name="reference">The <see cref="ReferencePackage"/> containing necessary references.</param>
    /// <returns>all <see cref="IPropertySymbol"/> and <see cref="IFieldSymbol"/> in top to bottom order of inheritance tree.</returns>
    internal static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol type, ReferencePackage reference) => type.GetAllMembersBottomToTop(reference);
    public static string GenericRestrictions(this INamedTypeSymbol namedType)
    {
        const string whereClause = "where ";
        var stringBuilder = new StringBuilder();
        foreach (var typeRestriction in namedType.TypeParameters)
        {
            var constraints = typeRestriction.ConstraintTypes.Select(restriction => restriction.ToDisplayString());
            List<string> restrictionsString = new();
            if (constraints.Any())
                restrictionsString.AddRange(constraints);
            if (typeRestriction.HasNotNullConstraint)
            {
                restrictionsString.Add("notnull");
            }
            if (typeRestriction.HasReferenceTypeConstraint)
            {
                restrictionsString.Add("class");
            }
            if (typeRestriction.HasUnmanagedTypeConstraint)
            {
                restrictionsString.Add("unmangaged");
            }
            if (typeRestriction.HasValueTypeConstraint)
            {
                restrictionsString.Add("struct");
            }
            if (typeRestriction.HasConstructorConstraint)
            {
                restrictionsString.Add("new()");
            }
            
            if (restrictionsString.Count > 0)
                stringBuilder.Append(typeRestriction.ToDisplayString()).Append(" : ").AppendLine(string.Join(", ", restrictionsString));
        }
        return stringBuilder.Length > 0 ? whereClause + stringBuilder : "";
    }
    /// <summary>
    /// Finds abstract/base classes in the inheritance hierarchy of the specified <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="INamedTypeSymbol"/> for which to find abstract classes.</param>
    /// <returns>A list of abstract/base classes in the inheritance hierarchy of the specified <see cref="INamedTypeSymbol"/>.</returns>
    public static ImmutableArray<INamedTypeSymbol> FindBase(this INamedTypeSymbol typeSymbol, ReferencePackage package)
    {
        var result = new List<INamedTypeSymbol>();
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (baseType.IsAbstract || baseType.TryGetAttribute(package.DataContractAttribute, out var d))
                result.Add(baseType);
            baseType = baseType.BaseType;
        }
        return ImmutableArray.Create(result.ToArray());
    }
    private static IEnumerable<ISymbol> GetAllMembersBottomToTop(this INamedTypeSymbol type, ReferencePackage reference)
    {
        // Get the base types in reverse order
        var baseTypes = GetBaseTypes(type, reference);
        List<string> properties = [];
        List<string> fields = [];
        foreach (var baseType in baseTypes)
        {
            // Get members of the base type in bottom to top order
            foreach (var member in (ImmutableArray<ISymbol>)baseType.GetMembers())
            {
                // Filter only properties and fields
                if (member is IPropertySymbol property)
                {
                    if (property.IsAbstract || properties.Contains(property.Name))
                    {
                        continue;
                    }
                    properties.Add(property.Name);
                    yield return property;
                }
                if (member is IFieldSymbol field)
                {
                    if (field.IsAbstract || fields.Contains(field.Name))
                    {
                        continue;
                    }
                    fields.Add(field.Name);
                    yield return field;
                }
            }
        }
    }

    /// <summary>
    /// Retrieves all base types of the specified <see cref="INamedTypeSymbol"/> in bottom to top order.
    /// </summary>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> for which to retrieve base types.</param>
    /// <returns>All base types in bottom to top order.</returns>
    private static IEnumerable<ITypeSymbol> GetBaseTypes(this INamedTypeSymbol type, ReferencePackage reference)
    {
        while (type != null)
        {
            // Check if the type has the specified DataContractAttribute
            if (type.GetAttributes().Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass.OriginalDefinition, reference.DataContractAttribute)))
                yield return type;

            type = type.BaseType;
        }
    }
    /// <summary>
    /// Converts a sequence of <see cref="ISymbol"/> to a sequence of <see cref="SymbolInfo"/>
    /// using the specified analyzers for <see cref="IFieldSymbol"/> and <see cref="IPropertySymbol"/>.
    /// </summary>
    /// <param name="type">The sequence of <see cref="ISymbol"/> to be converted.</param>
    /// <param name="references">The <see cref="ReferencePackage"/> containing necessary references.</param>
    /// <param name="propertyAnalyzers">The list of analyzers for <see cref="IPropertySymbol"/>.</param>
    /// <param name="fieldAnalyzers">The list of analyzers for <see cref="IFieldSymbol"/>.</param>
    /// <returns>An <see cref="IEnumerable{SymbolInfo}"/> representing analyzed information for each symbol.</returns>
    internal static IEnumerable<SymbolInfo> AsSymbolInfo(this IEnumerable<ISymbol> type, ReferencePackage references, List<IMemberSymbolAnalyzer<IPropertySymbol>> propertyAnalyzers, List<IMemberSymbolAnalyzer<IFieldSymbol>> fieldAnalyzers)
    {
        foreach (var symbol in type)
        {
            if (symbol is IPropertySymbol prop)
            {
                yield return propertyAnalyzers.Analyze(new MemberData<IPropertySymbol>(prop, DataMemberContext.Create(symbol, references)));
            }
            if (symbol is IFieldSymbol field)
            {
                yield return fieldAnalyzers.Analyze(new MemberData<IFieldSymbol>(field, DataMemberContext.Create(symbol, references)));
            }
        }
    }
    internal static ClassPackage ConvertToPackage(this INamedTypeSymbol namedTypeSymbol, ReferencePackage package, ImmutableArray<AttributeData> attributes)
    {
        var standardAssignAnalyzer = new PropertyAnalyzer(package)
            .HasVisibleGetter()
            .HasVisibleSetter()
            .IsNonStatic();
        var standardFieldAssignAnalyzer = new FieldAnalyzer(package)
            .IsVisibleToSerializer()
            .WhenNot(x => x.IsConst())
            .WhenNot(x => x.IsReadOnly())
            .IsNonStatic();

        List<IMemberSymbolAnalyzer<IFieldSymbol>> fieldAnalyzers = new()
        {
            standardFieldAssignAnalyzer
        };
        List<IMemberSymbolAnalyzer<IPropertySymbol>> propertyAnalyzers = new()
        {
            standardAssignAnalyzer
        };
        var members = namedTypeSymbol.GetAllMembers(package).AsSymbolInfo(package, propertyAnalyzers, fieldAnalyzers).Reduce();

        var memberList = ImmutableList.Create(members.ToArray());

        var datacontract = attributes.First(a => a.AttributeClass.Equals(package.DataContractAttribute, SymbolEqualityComparer.Default));

        return new ClassPackage(ClassInfo.CreateFrom(namedTypeSymbol, datacontract, package), memberList);
    }
}