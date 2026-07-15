using System.Text;
using NexYaml.Parser.Scopes;

namespace NexYaml.Parser
{
    public static class YamlDumpExtensions
    {
        public static void EmptyDump(this Scope scope)
        {
            if (scope.IsMapping)
            {
                foreach (var m in scope.AsMapping())
                {
                    m.Value.EmptyDump();
                }
            }
            if (scope.IsSequence)
            {
                foreach (var m in scope.AsSequence())
                {
                    m.Data.EmptyDump();
                }
            }
            if (scope.IsScalar)
            {
                scope.AsScalar();
            }
        }
        public static string Dump(this Scope scope, int indent = 0, bool includeHeader = true)
        {
            var pad = new string(' ', indent);
            string TagSuffix(string tag) => $"({tag})";
            if (scope.IsScalar)
            {
                return $"{pad}SCALAR(TODO)({scope.AsScalar()})";
            }
            else if(scope.IsMapping)
            {
                var sb = new StringBuilder();
                if (includeHeader)
                    sb.AppendLine($"{pad}MAPPING(TODO)");
                sb.AppendLine($"{pad}{{");
                foreach (var s in scope.AsMapping())
                {
                    if (s.Value.IsScalar)
                    {
                        sb.AppendLine($"{pad}  SCALAR({s.Key}) = SCALAR({s.Value.AsScalar()})");
                    }
                    else
                    {
                        var header = s.Value.GetType().Name.ToString().ToUpper();
                        sb.AppendLine($"{pad}  SCALAR({s.Key}) = {header}");
                        sb.Append(s.Value.Dump(indent + 2, includeHeader: false));
                    }
                }
                sb.AppendLine($"{pad}}}");
                return sb.ToString();
            }
            else if(scope.IsSequence)
            {
                var sb = new StringBuilder();
                if (includeHeader)
                    sb.AppendLine($"{pad}SEQUENCE");
                sb.AppendLine($"{pad}[");
                foreach (var item in scope.AsSequence())
                {
                    sb.AppendLine(item.Data.Dump(indent + 2, includeHeader: true));
                }
                sb.AppendLine($"{pad}]");
                return sb.ToString();
            }
            else
            {
                throw new Exception("???");
            }
        }
    }
}
