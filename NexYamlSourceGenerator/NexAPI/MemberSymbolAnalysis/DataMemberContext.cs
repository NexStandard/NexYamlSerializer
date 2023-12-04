using Microsoft.CodeAnalysis;
using StrideSourceGenerator.Core;
using StrideSourceGenerator.NexAPI.Core;
using System.Runtime.Serialization;

namespace StrideSourceGenerator.NexAPI.MemberSymbolAnalysis
{
    internal class DataMemberContext
    {
        private DataMemberContext() { }
        internal static DataMemberContext Create(ISymbol symbol, INamedTypeSymbol dataMemberIgnoreAttribute)
        {
            DataMemberContext context = new DataMemberContext();

            if (symbol.TryGetAttribute(dataMemberIgnoreAttribute, out AttributeData attributeData))
            {
                context.Exists = false;
            }
            else
            {
                context.Exists = true;
                context.Mode = 0;
                context.Order = 0;
            }
            return context;
        }
        public bool Exists { get; set; }
        public int Mode { get; set; }
        public int Order { get; set; }
    }
}