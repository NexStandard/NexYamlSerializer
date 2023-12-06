using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.MemberApi.ModeInfos;
using StrideSourceGenerator.NexAPI.MemberSymbolAnalysis;
using System.Collections.Immutable;
using System.Reflection;

internal class SymbolInfo
{
    public bool IsInterface { get; internal set; }
    internal virtual bool IsEmpty { get; } = false;
    internal string Name { get; set; }
    internal string Type { get; set; }
    internal string Namespace { get; set; }
    internal SymbolKind TypeKind { get; set; }
    internal IContentModeInfo MemberGenerator { get; set; }
    internal bool IsByteType { get; set; }
    internal bool IsArray { get; set; }
    internal bool IsGeneric { get; set; }
    internal bool IsAbstract { get; set; }
    internal string Tag {  get => $$"""emitter.Tag($"!{typeof({{Name}})},{{Namespace}}")""";}
    internal DataMemberContext Context { get; set; }
}