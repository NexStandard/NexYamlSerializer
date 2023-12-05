using NexYamlSourceGenerator.MemberApi.ModeInfos;

namespace NexYamlSourceGenerator.MemberApi.ModeInfos.Yaml
{
    internal class AssignModeInfo : IContentModeInfo
    {
        public bool IsContentMode { get; set; }
        public string GenerationInvocation { get; }
        public bool NeedsFinalAssignment { get; }
    }
}