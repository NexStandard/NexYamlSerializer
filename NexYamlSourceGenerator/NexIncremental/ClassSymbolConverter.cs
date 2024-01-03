using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using NexYamlSourceGenerator.NexAPI;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.FieldAnalyzers;
using NexYamlSourceGenerator.MemberApi.Analysation.PropertyAnalyzers;
using NexYamlSourceGenerator.MemberApi;

namespace NexYamlSourceGenerator.NexIncremental;

internal class ClassSymbolConverter
{
    internal ClassPackage Convert(INamedTypeSymbol namedTypeSymbol, ReferencePackage references, ImmutableArray<AttributeData> attributes)
    {
        var standardAssignAnalyzer = new PropertyAnalyzer()
            .HasVisibleGetter()
            .HasVisibleSetter();
        var standardFieldAssignAnalyzer = new FieldAnalyzer()
            .IsVisibleToSerializer();
        List<IMemberSymbolAnalyzer<IFieldSymbol>> fieldAnalyzers = new()
        {
            standardFieldAssignAnalyzer
        };
        List<IMemberSymbolAnalyzer<IPropertySymbol>> propertyAnalyzers = new()
        {
            standardAssignAnalyzer
        };
        var members = namedTypeSymbol.GetAllMembers(references).AsSymbolInfo(references, propertyAnalyzers, fieldAnalyzers).Reduce();
        
        var memberList  = ImmutableList.Create(members.ToArray());

        var datacontract = attributes.First(a => a.AttributeClass.Equals(references.DataContractAttribute, SymbolEqualityComparer.Default));

        return new ClassPackage()
        {
            ClassInfo = ClassInfo.CreateFrom(namedTypeSymbol,datacontract),
            MemberSymbols = memberList,
        };
    }
}