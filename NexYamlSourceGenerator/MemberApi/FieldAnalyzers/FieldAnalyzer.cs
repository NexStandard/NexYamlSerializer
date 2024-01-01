using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi.FieldAnalyzers;

internal class FieldAnalyzer : IMemberSymbolAnalyzer<IFieldSymbol>
{

    public SymbolInfo Analyze(MemberContext<IFieldSymbol> context)
    {
        var names = context.Symbol.Type.ContainingNamespace;
        string typeName;
        ITypeSymbol type;
        bool isArray = context.Symbol.Type.TypeKind == TypeKind.Array;
        if (isArray)
        {
            typeName = ((IArrayTypeSymbol)context.Symbol.Type).ElementType.Name;
            type = ((IArrayTypeSymbol)context.Symbol.Type).ElementType;
        }
        else
        {
            typeName = context.Symbol.Type.Name;
            type = context.Symbol.Type;
        }
        if (names != null)
        {
            typeName = type.GetFullNamespace('.') + "." + typeName;
        }

        return new SymbolInfo()
        {
            Name = context.Symbol.Name,
            TypeKind = SymbolKind.Field,
            Type = typeName,
            IsAbstract = context.Symbol.Type.IsAbstract,
            IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
            Context = context.DataMemberContext,
            IsByteType = context.Symbol.Type.SpecialType is SpecialType.System_Byte or SpecialType.System_SByte,
            IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,

        };
    }

    public bool AppliesTo(MemberContext<IFieldSymbol> symbol) => true;
}