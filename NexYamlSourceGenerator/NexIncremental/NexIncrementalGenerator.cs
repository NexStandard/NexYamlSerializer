using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NexYamlSourceGenerator.NexAPI;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.Templates;
using NexYamlSourceGenerator.MemberApi;
using System.Diagnostics;

namespace NexYamlSourceGenerator.NexIncremental;

[Generator]
internal class NexIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classProvider = context.SyntaxProvider.ForAttributeWithMetadataName(ReferencePackage.DataContract,
            (node, transform) => node is TypeDeclarationSyntax,
            (ctx, transform) =>
            {
                var classDeclaration = (ITypeSymbol)ctx.TargetSymbol;
                var semanticModel = ctx.SemanticModel;
                var compilation = semanticModel.Compilation;
                var package = new ReferencePackage(compilation);
                if (!package.IsValid())
                    return null;
                return new ClassSymbolConverter().Convert(classDeclaration, package);
            }
        );

        context.RegisterSourceOutput(classProvider, Generate);
    }

    private void Generate(SourceProductionContext context, ClassPackage info)
    {
        context.AddSource(info.ClassInfo.GeneratorName + ".g.cs", info.ConvertToSourceCode());
    }
}
