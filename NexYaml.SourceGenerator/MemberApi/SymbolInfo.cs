using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.MemberApi.Data;

namespace NexYaml.SourceGenerator.MemberApi;
internal record SymbolInfo
{
    public static SymbolInfo Empty { get; } = new SymbolInfo() { IsEmpty = true };
    public bool IsInterface { get; init; }
    public bool IsInit { get; init; }
    public bool IsReadonly { get; internal set; }
    internal virtual bool IsEmpty { get; init; } = false;
    internal bool IsHidden { get; init; } = false;
    internal string Name { get; init; }
    internal string Type { get; init; }
    internal SymbolKind TypeKind { get; init; }
    internal bool IsArray { get; init; }
    internal bool IsAbstract { get; init; }
    internal string DataStyle { get; init; }
    internal DataMemberContext Context { get; init; }
}
