using Stride.Core;

namespace NexYamlTest.DataMemberContentMode;
[DataContract]
internal class ContentModeClass
{
    [DataMember(DataMemberMode.Content)]
    public ContentModeData Content { get; set; } = new ContentModeData(12);
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