using StrideSourceGenerator.NexAPI;

namespace StrideSourceGenerator.ModeInfos.Yaml
{
    internal class CollectionContentModeInfo : IContentModeInfo
    {
        public string TempVariable { get; }
        public string GenerationInvocation { get; }
        public bool NeedsFinalAssignment { get; }
        public bool IsContentMode { get; set; }
    }
}