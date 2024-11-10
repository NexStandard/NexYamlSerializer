using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;

namespace NexYamlSourceGenerator.MemberApi.Data;

internal record MemberData<T>(T Symbol, DataMemberContext DataMemberContext) where T : ISymbol;

internal record ClassPackage(ClassInfo ClassInfo, EquatableReadOnlyList<SymbolInfo> MemberSymbols);

internal record DataPackage(string DisplayString, string ShortDisplayString, bool IsGeneric, string[] TypeParameters);