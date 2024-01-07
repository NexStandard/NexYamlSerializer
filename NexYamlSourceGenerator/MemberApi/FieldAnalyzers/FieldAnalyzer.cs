using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.Analyzers;
using NexYamlSourceGenerator.MemberApi.Data;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class FieldAnalyzer : IMemberSymbolAnalyzer<IFieldSymbol>
{
    public SymbolInfo Analyze(Data<IFieldSymbol> context)
    {
        var typeBundle = GetTypeDisplay(context.Symbol.Type);

        return new SymbolInfo()
        {
            Name = context.Symbol.Name,
            TypeKind = SymbolKind.Field,
            Type = typeBundle,
            IsAbstract = context.Symbol.Type.IsAbstract,
            IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
            Context = context.DataMemberContext,
            IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,
            IsHidden = context.DataMemberContext.IsHidden,
        };
    }

    public bool AppliesTo(Data<IFieldSymbol> symbol) => true;

    string GetTypeDisplay(ITypeSymbol type)
    => type.TypeKind == TypeKind.Array ?
        ((IArrayTypeSymbol)type).ElementType.ToDisplayString() : type.ToDisplayString();
}