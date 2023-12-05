using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StrideSourceGenerator.Core;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using NexYamlSourceGenerator.NexAPI;
using NexYamlSourceGenerator.MemberApi;
using NexYamlSourceGenerator.MemberApi.Analysation.Analyzers;
using NexYamlSourceGenerator.MemberApi.ModeInfos.Yaml;
using NexYamlSourceGenerator.NexIncremental;

namespace StrideSourceGenerator.NexIncremental
{
    [Generator]
    internal class NexIncrementalGenerator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Debugger.Launch();
            AssignModeInfo assignModeInfo = new AssignModeInfo();
            IncrementalValueProvider<ImmutableArray<ClassPackage>> classProvider = context.SyntaxProvider
                                       .CreateSyntaxProvider((node, transform) =>
                                       {
                                           return node is TypeDeclarationSyntax;
                                       },
                                       (ctx, transform) =>
                                       {
                                           TypeDeclarationSyntax classDeclaration = (TypeDeclarationSyntax)ctx.Node;
                                           Compilation compilation = ctx.SemanticModel.Compilation;
                                           SemanticModel semanticModel = ctx.SemanticModel;
                                           return CreateClassInfo(compilation, classDeclaration, semanticModel);
                                       })
                                       .Where(x => x is not null)
                                       .Collect();

            context.RegisterSourceOutput(classProvider, Generate);
        }

        private ClassPackage CreateClassInfo(Compilation compilation, TypeDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            INamedTypeSymbol dataContractAttribute = WellKnownReferences.DataContractAttribute(compilation);

            if (dataContractAttribute is null)
                return null;

            ITypeSymbol type = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (!type.HasAttribute(dataContractAttribute))
                return null;
            MemberSelector memberSelector = new MemberSelector(dataContractAttribute);
            AssignModeInfo assignMode = new AssignModeInfo();

            IMemberSymbolAnalyzer<IPropertySymbol> standardAssignAnalyzer = new PropertyAnalyzer(assignMode)
                .HasVisibleGetter()
                .HasVisibleSetter();
            IMemberSymbolAnalyzer<IFieldSymbol> standardFieldAssignAnalyzer = new FieldAnalyzer(assignMode)
                .IsVisibleToSerializer();

            MemberProcessor classInfoMemberProcessor = new MemberProcessor(memberSelector, compilation)
                .Attach(standardAssignAnalyzer)
                .Attach(standardFieldAssignAnalyzer);
            var members = classInfoMemberProcessor.Process(type);

            return new ClassPackage()
            {
                ClassInfo = ClassInfo.CreateFrom(type),
                MemberSymbols = members,
            };
        }

        private static void Generate(
          SourceProductionContext ctx,
          ImmutableArray<ClassPackage> myCustomObjects)
        {
            foreach (ClassPackage obj in myCustomObjects)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                Generates(ctx, obj);
            }
        }
        private static SourceCreator SourceCreator = new SourceCreator();
        private static void Generates(SourceProductionContext ctx, ClassPackage info)
        {
            if (info is null)
                return;

            ctx.AddSource(info.ClassInfo.GeneratorName + ".g.cs", SourceCreator.Create(ctx, info));
        }
    }
}