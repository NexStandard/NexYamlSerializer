using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi;

internal record SymbolInfo
{
    public static SymbolInfo Empty { get; } = new SymbolInfo() { IsEmpty = true };
    public bool IsInterface { get; internal set; }
    internal virtual bool IsEmpty { get; set; } = false;
    internal string Name { get; set; }
    internal string Type { get; set; }
    internal string Namespace { get; set; }
    internal SymbolKind TypeKind { get; set; }
    internal bool IsByteType { get; set; }
    internal bool IsArray { get; set; }
    internal bool IsGeneric { get; set; }
    internal bool IsAbstract { get; set; }
    internal string Tag {  get => $$"""emitter.Tag($"!{typeof({{Name}})},{{Namespace}}")""";}
    internal DataMemberContext Context { get; set; }
}
