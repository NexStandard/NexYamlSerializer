
namespace NexYaml.Serialization;
public class DelegateWriter(IYamlSerializerResolver resolver, WriteDelegate write) : Writer(resolver)
{
    public override void Write(ReadOnlySpan<char> text)
    {
        write.Invoke(text);
    }
}
