using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.NexAPI;

namespace NexYamlSourceGenerator.NexIncremental
{
    internal class MemberSelector() : IMemberSelector
    {
        public IEnumerable<ISymbol> GetAllMembers(ITypeSymbol type)
        {
            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol || type is IFieldSymbol)
                    yield return member;
            }
        }
    }
}