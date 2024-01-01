using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.NexAPI;
using System.Linq;

namespace NexYamlSourceGenerator.NexIncremental;

internal static class TypeSymbolExtensions
{
    /// <summary>
    /// Retrieves all <see cref="IPropertySymbol"/> and <see cref="IFieldSymbol"/> from the specified <see cref="ITypeSymbol"/> and it's base types,
    /// returning them in reverse order of inheritance ( top to bottom ).
    /// </summary>
    /// <param name="type">The <see cref="ITypeSymbol"/> to retrieve members for.</param>
    /// <param name="reference">The <see cref="ReferencePackage"/> containing necessary references.</param>
    /// <returns>all <see cref="IPropertySymbol"/> and <see cref="IFieldSymbol"/> in top to bottom order of inheritance tree.</returns>
    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type, ReferencePackage reference)
    {
        // Get the base types in reverse order
        var baseTypes = GetBaseTypes(type, reference).Reverse();

        foreach (var baseType in baseTypes)
        {
            // Get members of the base type in reverse order
            var members = baseType.GetMembers();

            foreach (var member in members)
            {
                // Filter only properties and fields
                if (member is IPropertySymbol or IFieldSymbol)
                    yield return member;
            }
        }
    }

    /// <summary>
    /// Retrieves all base types of the specified <see cref="ITypeSymbol"/> in bottom to top order.
    /// </summary>
    /// <param name="type">The <see cref="ITypeSymbol"/> for which to retrieve base types.</param>
    /// <returns>All base types in bottom to top order.</returns>
    private static IEnumerable<ITypeSymbol> GetBaseTypes(this ITypeSymbol type, ReferencePackage reference)
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
    public static IEnumerable<SymbolInfo> AsSymbolInfo(this IEnumerable<ISymbol> type, ReferencePackage references, List<IMemberSymbolAnalyzer<IPropertySymbol>> propertyAnalyzers, List<IMemberSymbolAnalyzer<IFieldSymbol>> fieldAnalyzers)
    {
        foreach (var symbol in type)
        {
            if (symbol is IPropertySymbol prop)
            {
                yield return propertyAnalyzers.Analyze(new MemberContext<IPropertySymbol>(prop, DataMemberContext.Create(symbol, references)));
            }
            if (symbol is IFieldSymbol field)
            {
                yield return fieldAnalyzers.Analyze(new MemberContext<IFieldSymbol>(field, DataMemberContext.Create(symbol, references)));
            }
        }
    }
}