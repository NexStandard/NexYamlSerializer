#nullable enable
namespace NexVYaml.Parser;

readonly struct VersionDirective(int major, int minor) : ITokenContent
{
    public readonly int Major = major;
    public readonly int Minor = minor;
}

