using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;

internal class PropertyAnalyzer(ReferencePackage package) : IMemberSymbolAnalyzer<IPropertySymbol>
{
    public SymbolInfo Analyze(MemberData<IPropertySymbol> context)
    {
        var typeName = GetTypeDisplay(context.Symbol.Type);
        var dataStyle = new DataStyleAnalyzer(context.Symbol, package).Analyze();
        return new SymbolInfo()
        {
            Name = context.Symbol.Name,
            TypeKind = SymbolKind.Property,
            DataStyle = dataStyle,
            IsAbstract = context.Symbol.Type.IsAbstract,
            IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
            Type = typeName,
            Context = context.DataMemberContext,
            IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,
        };
    }

    public bool AppliesTo(MemberData<IPropertySymbol> symbol)
    {
        return true;
    }

    private string GetTypeDisplay(ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Array ?
            ((IArrayTypeSymbol)type).ElementType.ToDisplayString() : type.ToDisplayString();
    }
}