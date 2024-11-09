using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class FieldAnalyzer(ReferencePackage package) : IMemberSymbolAnalyzer<IFieldSymbol>
{
    public SymbolInfo Analyze(MemberData<IFieldSymbol> context)
    {
        var typeBundle = GetTypeDisplay(context.Symbol.Type);

        return new SymbolInfo()
        {
            Name = context.Symbol.Name,
            TypeKind = SymbolKind.Field,
            DataStyle = new DataStyleAnalyzer(context.Symbol, package).Analyze(),
            Type = typeBundle,
            IsAbstract = context.Symbol.Type.IsAbstract,
            IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
            Context = context.DataMemberContext,
            IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,
        };
    }

    public bool AppliesTo(MemberData<IFieldSymbol> symbol) => true;

    string GetTypeDisplay(ITypeSymbol type)
    => type.TypeKind == TypeKind.Array ?
        ((IArrayTypeSymbol)type).ElementType.ToDisplayString() : type.ToDisplayString();
}