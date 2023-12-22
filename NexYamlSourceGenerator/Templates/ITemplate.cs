using NexYamlSourceGenerator.NexAPI;

namespace NexYamlSourceGenerator.Templates
{
    internal interface ITemplate
    {
        public string Create(ClassPackage info);
    }
}