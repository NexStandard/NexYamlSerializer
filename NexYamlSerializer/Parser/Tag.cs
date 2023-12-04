#nullable enable
using System;

namespace VYaml.Parser
{
    public class Tag : ITokenContent
    {
        public string Prefix { get; }
        public string Handle { get; }

        public Tag(string prefix, string handle)
        {
            Prefix = prefix;
            Handle = handle;
        }

        public override string ToString() => $"{Prefix}{Handle}";

        public bool Equals(string tagString)
        {
            if (tagString.Length != Prefix.Length + Handle.Length)
            {
                return false;
            }
            var handleIndex = tagString.IndexOf(Prefix, StringComparison.Ordinal);
            if (handleIndex < 0)
            {
                return false;
            }
            return tagString.IndexOf(Handle, handleIndex, StringComparison.Ordinal) > 0;
        }
    }
}

