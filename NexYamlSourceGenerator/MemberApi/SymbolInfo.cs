using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.MemberApi;
internal record SymbolInfo
{
    public static SymbolInfo Empty { get; } = new SymbolInfo() { IsEmpty = true };
    public bool IsInterface { get; init; }
    internal virtual bool IsEmpty { get; init; } = false;
    internal bool IsHidden { get; init; } = false;
    internal string Name { get; init; }
    internal string Type { get; init; }
    internal string Namespace { get; init; }
    internal SymbolKind TypeKind { get; init; }
    internal bool IsArray { get; init; }
    internal bool IsAbstract { get; init; }
    internal DataMemberContext Context { get; init; }
}
