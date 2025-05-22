using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.Core;
using NexYaml.SourceGenerator.MemberApi.Analyzers;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi.PropertyAnalyzers;

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
            IsRequired = context.Symbol.IsRequired,
            IsInit = context.Symbol.SetMethod?.IsInitOnly ?? false,
            IsReadonly = context.Symbol.SetMethod == null || context.Symbol.SetMethod.DeclaredAccessibility is not Accessibility.Public or Accessibility.Internal
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