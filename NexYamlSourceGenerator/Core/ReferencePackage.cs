using Microsoft.CodeAnalysis;

namespace NexYamlSourceGenerator.Core;

internal class ReferencePackage(Compilation compilation)
{
    public INamedTypeSymbol DataMemberAttribute { get; } =
        compilation.GetTypeByMetadataName("Stride.Core.DataMemberAttribute");

    public INamedTypeSymbol DataMemberIgnoreAttribute { get; } =
        compilation.GetTypeByMetadataName("Stride.Core.DataMemberIgnoreAttribute");

    public INamedTypeSymbol DataContractAttribute { get; } =
         compilation.GetTypeByMetadataName("Stride.Core.DataContractAttribute");

    public INamedTypeSymbol DataStyleAttribute { get; } =
         compilation.GetTypeByMetadataName("Stride.Core.DataStyleAttribute");

    public INamedTypeSymbol DataStyle { get; } =
     compilation.GetTypeByMetadataName("Stride.Core.DataStyle");
    public bool IsValid()
    {
        return DataMemberAttribute != null && DataMemberIgnoreAttribute != null && DataContractAttribute != null;
    }
}