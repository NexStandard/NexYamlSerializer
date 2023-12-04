using Microsoft.CodeAnalysis;
using StrideSourceGenerator.NexAPI.Core;
using System.Runtime.Serialization;

namespace StrideSourceGenerator.NexAPI.MemberSymbolAnalysis
{
    internal class DataMemberContext
    {
        private DataMemberContext() { }
        internal static DataMemberContext Create(ISymbol symbol, INamedTypeSymbol dataMemberAttribute)
        {
            DataMemberContext context = new DataMemberContext();
            if (symbol.TryGetAttribute(dataMemberAttribute, out AttributeData attributeData))
            {
                context.Exists = true;
                context.Mode = 0;
                context.Order = 0;
            }
            else
            {
                context.Exists = false;
            }
            return context;
        }
        public bool Exists { get; set; }
        public int Mode { get; set; }
        public int Order { get; set; }
    }
}