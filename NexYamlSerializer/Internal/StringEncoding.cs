using System.Text;

namespace NexVYaml.Internal;

static class StringEncoding
{
    public static readonly Encoding Utf8 = new UTF8Encoding(false);
}
