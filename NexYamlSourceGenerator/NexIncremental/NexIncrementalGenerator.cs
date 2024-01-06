using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.Templates;
using NexYamlSourceGenerator.MemberApi;

namespace NexYamlSourceGenerator.NexIncremental;

[Generator]
internal class NexIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classProvider = context.SyntaxProvider.ForAttributeWithMetadataName(ReferencePackage.DataContract,
            (node, transform) => node is TypeDeclarationSyntax n && !n.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)),
            (ctx, transform) =>
            {
                if (ctx.TargetSymbol is not INamedTypeSymbol classDeclaration)
                    return null;

                var semanticModel = ctx.SemanticModel;
                var attributes = ctx.Attributes;
                var compilation = semanticModel.Compilation;
                var references = new ReferencePackage(compilation);
                if (!references.IsValid())
                    return null;
                return classDeclaration.ConvertToPackage(references, attributes);
            }
        );

        context.RegisterSourceOutput(classProvider, Generate);
    }

    private void Generate(SourceProductionContext context, ClassPackage info)
    {
        context.AddSource(info.ClassInfo.GeneratorName + ".g.cs", info.ConvertToSourceCode());
    }
}
