namespace NexYamlSourceGenerator.MemberApi.ModeInfos
{
    internal interface IContentModeInfo
    {
        public bool IsContentMode { get; set; }
        public string GenerationInvocation { get; }
        public bool NeedsFinalAssignment { get; }
    }
}