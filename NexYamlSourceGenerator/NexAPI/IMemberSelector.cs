using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.NexAPI
{
    internal interface IMemberSelector
    {
        IEnumerable<ISymbol> GetAllMembers(ITypeSymbol type);
    }
}