namespace NexYaml.Parser;

internal readonly record struct VersionDirective(int major, int minor) : ITokenContent
{
    public readonly int Major = major;
    public readonly int Minor = minor;
}
