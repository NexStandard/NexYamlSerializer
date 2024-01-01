using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.NexAPI;
using System.Linq;

namespace NexYamlSourceGenerator.NexIncremental;

internal static class TypeSymbolExtensions
{
    public static IEnumerable<T> GetAllMembers<T>(this ITypeSymbol type)
        where T : ISymbol
    {
        foreach(var member in type.GetMembers())
        {
            if (member is T temp)
                yield return temp;
        }
    }
    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
    {
        foreach (var member in type.GetMembers())
        {
            if (member is IPropertySymbol or IFieldSymbol)
                yield return member;
        }
    }
    public static IEnumerable<SymbolInfo> AsSymbolInfo(this IEnumerable<ISymbol> type, ReferencePackage references, List<IMemberSymbolAnalyzer<IPropertySymbol>> propertyAnalyzers, List<IMemberSymbolAnalyzer<IFieldSymbol>> fieldAnalyzers)
    {
        foreach(var  symbol in type)
        {
            if(symbol is IPropertySymbol prop)
            {
                yield return propertyAnalyzers.Analyze(new MemberContext<IPropertySymbol>(prop, DataMemberContext.Create(symbol,references)));
            }
            if(symbol is IFieldSymbol field)
            {
                yield return fieldAnalyzers.Analyze(new MemberContext<IFieldSymbol>(field, DataMemberContext.Create(symbol, references)));
            }
        }
    }

    static MemberContext<T> AsMemberContext<T>(this T member, ReferencePackage references)
        where T : ISymbol
        => new (member, DataMemberContext.Create(member, references));
}