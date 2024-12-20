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
            IsInit = context.Symbol.IsReadOnly,
        };
    }

    public bool AppliesTo(MemberData<IFieldSymbol> symbol)
    {
        return true;
    }

    private string GetTypeDisplay(ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Array ?
            ((IArrayTypeSymbol)type).ElementType.ToDisplayString() : type.ToDisplayString();
    }
}