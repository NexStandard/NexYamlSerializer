using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi
{
    internal class MemberContext<T>
        where T : ISymbol
    {

        public MemberContext(T symbol, DataMemberContext context)
        {
            Symbol = symbol;
            DataMemberContext = context;
        }
        public T Symbol { get; }
        public DataMemberContext DataMemberContext { get; }
    }
}