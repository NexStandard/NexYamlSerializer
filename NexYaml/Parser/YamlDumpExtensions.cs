using System.Text;

namespace NexYaml.Parser
{
    public static class YamlDumpExtensions
    {
        public static string Dump(this Scope scope, int indent = 0, bool includeHeader = true)
        {
            var pad = new string(' ', indent);
            string TagSuffix(string tag) => $"({tag})";

            switch (scope)
            {
                case ScalarScope s:
                    return $"{pad}SCALAR{TagSuffix(s.Tag)}({s.Value})";

                case MappingScope m:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}MAPPING{TagSuffix(m.Tag)}");
                        sb.AppendLine($"{pad}{{");
                        foreach (var (key, val) in m)
                        {
                            if (val is ScalarScope scalar)
                            {
                                sb.AppendLine($"{pad}  SCALAR({key}) = SCALAR{TagSuffix(scalar.Tag)}({scalar.Value})");
                            }
                            else
                            {
                                var header = val.Kind.ToString().ToUpper();
                                sb.AppendLine($"{pad}  SCALAR({key}) = {header}{TagSuffix(val.Tag)}");
                                sb.Append(val.Dump(indent + 2, includeHeader: false));
                            }
                        }
                        sb.AppendLine($"{pad}}}");
                        return sb.ToString();
                    }

                case SequenceScope seq:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}SEQUENCE{TagSuffix(seq.Tag)}");
                        sb.AppendLine($"{pad}[");
                        foreach (var item in seq)
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
