using System.Text;
using NexYaml.Parser.Scopes;

namespace NexYaml.Parser
{
    public static class YamlDumpExtensions
    {
        public static void EmptyDump(this Scope scope)
        {
            if (scope.Kind is ScopeKind.BlockMapping or ScopeKind.FlowMapping or ScopeKind.PrefixedBlockMapping)
            {
                foreach (var m in scope.AsMapping())
                {
                    m.Value.EmptyDump();
                }
            }
            if (scope.Kind is ScopeKind.BlockSequence or ScopeKind.FlowSequence)
            {
                foreach (var m in scope.AsSequence())
                {
                    m.EmptyDump();
                }
            }
        }
        public static string Dump(this Scope scope, int indent = 0, bool includeHeader = true)
        {
            var pad = new string(' ', indent);
            string TagSuffix(string tag) => $"({tag})";

            switch (scope.Kind)
            {
                case ScopeKind.Scalar or ScopeKind.LazyScalar or ScopeKind.NullScalar:
                    return $"{pad}SCALAR{TagSuffix(scope.Tag)}({scope.AsScalar()})";

                case ScopeKind.BlockMapping or ScopeKind.FlowMapping or ScopeKind.PrefixedBlockMapping:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}MAPPING{TagSuffix(scope.Tag)}");
                        sb.AppendLine($"{pad}{{");
                        foreach (var s in scope.AsMapping())
                        {
                            if (s.Value.Kind is ScopeKind.Scalar)
                            {
                                sb.AppendLine($"{pad}  SCALAR({s.Key}) = SCALAR{TagSuffix(s.Value.Tag)}({s.Value.AsScalar()})");
                            }
                            else
                            {
                                var header = s.Value.Kind.ToString().ToUpper();
                                sb.AppendLine($"{pad}  SCALAR({s.Key}) = {header}{TagSuffix(s.Value.Tag)}");
                                sb.Append(s.Value.Dump(indent + 2, includeHeader: false));
                            }
                        }
                        sb.AppendLine($"{pad}}}");
                        return sb.ToString();
                    }

                case ScopeKind.FlowSequence or ScopeKind.BlockSequence:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}SEQUENCE{TagSuffix(scope.Tag)}");
                        sb.AppendLine($"{pad}[");
                        foreach (var item in scope.AsSequence())
                        {
                            sb.AppendLine(item.Dump(indent + 2, includeHeader: true));
                        }
                        sb.AppendLine($"{pad}]");
                        return sb.ToString();
                    }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
