using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.PropertyAnalyzers;

internal class PropertyAnalyzer(ReferencePackage package) : IMemberSymbolAnalyzer<IPropertySymbol>
{
    public SymbolInfo Analyze(Data<IPropertySymbol> context)
    {
        var typeName = GetTypeDisplay(context.Symbol.Type);
        return new SymbolInfo()
        {
            Name = context.Symbol.Name,
            TypeKind = SymbolKind.Property,
            DataStyle = new DataStyleAnalyzer(context.Symbol, package).Analyze(),
            IsAbstract = context.Symbol.Type.IsAbstract,
            IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
            Type = typeName,
            Context = context.DataMemberContext,
            IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,
        };
    }

    public bool AppliesTo(Data<IPropertySymbol> symbol) => true;
    string GetTypeDisplay(ITypeSymbol type)
        => type.TypeKind == TypeKind.Array ?
        ((IArrayTypeSymbol)type).ElementType.ToDisplayString() : type.ToDisplayString();
}