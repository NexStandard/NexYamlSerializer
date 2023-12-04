using StrideSourceGenerator.NexAPI;

namespace StrideSourceGenerator.ModeInfos.Yaml
{
    internal class AssignModeInfo : IContentModeInfo
    {
        public bool IsContentMode { get; set; }
        public string GenerationInvocation { get; }
        public bool NeedsFinalAssignment { get; }
    }
}