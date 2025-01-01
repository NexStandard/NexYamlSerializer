using Microsoft.CodeAnalysis;

namespace NexYaml.SourceGenerator.Core;

internal class ReferencePackage(Compilation compilation)
{
    public const string DataContract = "Stride.Core.DataContractAttribute";
    public INamedTypeSymbol DataMemberAttribute { get; } =
        compilation.GetTypeByMetadataName("Stride.Core.DataMemberAttribute");
    public bool IsDataMemberAttribute(ITypeSymbol typeSymbol)
    {
        return typeSymbol is
        {
            MetadataName: "DataMemberAttribute",
            ContainingNamespace:
            {
                Name: "Core",
                ContainingNamespace:
                {
                    Name: "Stride",
                    ContainingNamespace.IsGlobalNamespace: true,
                },
            },
        };
    }

    public INamedTypeSymbol DataMemberIgnoreAttribute { get; } =
        compilation.GetTypeByMetadataName("Stride.Core.DataMemberIgnoreAttribute");

    public INamedTypeSymbol DataContractAttribute { get; } =
         compilation.GetTypeByMetadataName("Stride.Core.DataContractAttribute");

    public INamedTypeSymbol DataStyleAttribute { get; } =
         compilation.GetTypeByMetadataName("Stride.Core.DataStyleAttribute");

    public INamedTypeSymbol DataStyle { get; } =
     compilation.GetTypeByMetadataName("Stride.Core.DataStyle");
    /// <summary>
    /// Checks whether the package is valid by ensuring that all necessary types are available.
    /// </summary>
    /// <returns><c>true</c> if the package is valid; otherwise, <c>false</c>.</returns>
    public bool IsValid()
    {
        return DataMemberAttribute != null && DataMemberIgnoreAttribute != null && DataContractAttribute != null;
    }
}