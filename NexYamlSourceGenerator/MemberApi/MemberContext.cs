using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.MemberApi;

internal record MemberContext<T>(T Symbol, DataMemberContext DataMemberContext) where T : ISymbol;