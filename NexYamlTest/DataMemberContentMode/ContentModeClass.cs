using Stride.Core;

namespace NexYamlTest.DataMemberContentMode;
[DataContract]
internal class ContentModeClass
{
    [DataMember(DataMemberMode.Content)]
    public ContentModeData Content { get; set; } = new ContentModeData(12);
    private ContentModeData ContentMode { get; set; } = new ContentModeData(12);
    [DataMember(DataMemberMode.Content)]
    public ContentModeData ContentInit { get => ContentMode; set { throw new System.Exception(); } }
    [DataMember]
    public required ContentModeData ContentInitRequired { get; set; } = new ContentModeData(12);
}
[DataContract]
internal class ContentModeData
{
    public ContentModeData() { }
    public ContentModeData(int generics)
    {
        Generics = generics;
    }

    [DataMember(DataMemberMode.Content)]
    public int Generics { get; set; } = 10;
}