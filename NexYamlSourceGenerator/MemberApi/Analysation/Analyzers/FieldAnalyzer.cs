using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using NexYamlSourceGenerator.MemberApi.ModeInfos;

namespace NexYamlSourceGenerator.MemberApi.Analysation.Analyzers
{
    internal class FieldAnalyzer : IMemberSymbolAnalyzer<IFieldSymbol>
    {
        protected readonly IContentModeInfo memberGenerator;
        internal FieldAnalyzer(IContentModeInfo memberGenerator)
        {
            this.memberGenerator = memberGenerator;
        }

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
                MemberGenerator = memberGenerator,
                Type = typeName,
                IsAbstract = context.Symbol.Type.IsAbstract,
                IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
                Context = context.DataMemberContext,
                IsByteType = context.Symbol.Type.SpecialType == SpecialType.System_Byte || context.Symbol.Type.SpecialType == SpecialType.System_SByte,
                IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,

            };
        }

        public bool AppliesTo(MemberContext<IFieldSymbol> symbol) => true;
    }
}