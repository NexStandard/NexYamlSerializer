using Microsoft.CodeAnalysis;
using StrideSourceGenerator.Core;
using StrideSourceGenerator.NexAPI;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;
using System.Collections.Immutable;

namespace NexYamlSourceGenerator.MemberApi
{
    internal class MemberProcessor(IMemberSelector selector, Compilation compilation)
    {
        List<IMemberSymbolAnalyzer<IPropertySymbol>> PropertyAnalyzers { get; set; } = new();
        List<IMemberSymbolAnalyzer<IFieldSymbol>> FieldAnalyzers { get; set; } = new();
        INamedTypeSymbol DataMemberIgnoreAttribute => WellKnownReferences.DataMemberIgnoreAttribute(compilation);
        internal ImmutableList<SymbolInfo> Process(ITypeSymbol type)
        {
            IReadOnlyList<ISymbol> symbols = selector.GetAllMembers(type);
            List<SymbolInfo> result = new List<SymbolInfo>();
            foreach (ISymbol symbol in symbols)
            {
                if (symbol == null)
                    continue;
                DataMemberContext context = DataMemberContext.Create(symbol, DataMemberIgnoreAttribute);
                if (symbol is IPropertySymbol property)
                    ProcessAnalyzers(PropertyAnalyzers, property, result, context);
                else if (symbol is IFieldSymbol field)
                {
                    ProcessAnalyzers(FieldAnalyzers, field, result, context);
                }
            }
            return ImmutableList.Create(result.ToArray());
        }
        internal MemberProcessor Attach(params IMemberSymbolAnalyzer<IPropertySymbol>[] analyzer)
        {
            PropertyAnalyzers.AddRange(analyzer);
            return this;
        }
        internal MemberProcessor Attach(params IMemberSymbolAnalyzer<IFieldSymbol>[] analyzer)
        {
            FieldAnalyzers.AddRange(analyzer);
            return this;
        }
        void ProcessAnalyzers<T>(List<IMemberSymbolAnalyzer<T>> analyzers, T symbol, List<SymbolInfo> result, DataMemberContext context)
            where T : ISymbol
        {
            if (context.Exists == false)
                return;
            foreach (IMemberSymbolAnalyzer<T> analyzer in analyzers)
            {
                MemberContext<T> memberContext = new MemberContext<T>(symbol, context);

                SymbolInfo temp = analyzer.Analyze(memberContext);
                if (!temp.IsEmpty)
                    result.Add(temp);
            }
        }
    }
}