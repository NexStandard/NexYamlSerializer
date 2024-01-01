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
    internal ClassPackage Convert(ITypeSymbol namedTypeSymbol, ReferencePackage references)
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
        return new ClassPackage()
        {
            ClassInfo = ClassInfo.CreateFrom(namedTypeSymbol),
            MemberSymbols = memberList,
        };
    }
}