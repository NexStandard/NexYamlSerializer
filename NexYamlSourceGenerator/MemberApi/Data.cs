using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi;

internal record Data<T>(T Symbol, DataMemberContext DataMemberContext) where T : ISymbol;

internal record ClassPackage(ClassInfo ClassInfo, ImmutableList<SymbolInfo> MemberSymbols);