using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.Core;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.NexAPI.Analysation.Analyzers
{
    internal class PropertyAnalyzer : IMemberSymbolAnalyzer<IPropertySymbol>
    {
        protected readonly IContentModeInfo memberGenerator;
        internal PropertyAnalyzer(IContentModeInfo memberGenerator)
        {
            this.memberGenerator = memberGenerator;
        }

        public SymbolInfo Analyze(MemberContext<IPropertySymbol> context)
        {
            string typeName;
            ITypeSymbol type;
            bool isArray = context.Symbol.Type.TypeKind == TypeKind.Array;
            if (isArray)
            {
                typeName = ((IArrayTypeSymbol)context.Symbol.Type).ElementType.ToDisplayString();
            }
            else
            {
                typeName = context.Symbol.Type.ToDisplayString();
            }
            return new SymbolInfo()
            {
                Name = context.Symbol.Name,
                TypeKind = SymbolKind.Property,
                IsAbstract = context.Symbol.Type.IsAbstract,
                IsInterface = context.Symbol.Type.TypeKind == TypeKind.Interface,
                MemberGenerator = memberGenerator,
                Type = typeName,
                Context = context.DataMemberContext,
                IsByteType = context.Symbol.Type.SpecialType == SpecialType.System_Byte || context.Symbol.Type.SpecialType == SpecialType.System_SByte,
                IsArray = context.Symbol.Type.TypeKind == TypeKind.Array,
            };
        }

        public bool AppliesTo(MemberContext<IPropertySymbol> symbol) => true;
    }
}