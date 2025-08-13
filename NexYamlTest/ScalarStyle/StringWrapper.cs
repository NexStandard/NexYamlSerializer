using Stride.Core;

namespace NexYamlTest.ScalarStyle
{
    [DataContract]
    internal class StringWrapper
    {
        public string? Value { get; set; }
    }
    [DataContract]
    internal class StringWrapperSecondProperty
    {
        public string? Value { get; set; }
        public int Secondary { get; set; }
    }
}
