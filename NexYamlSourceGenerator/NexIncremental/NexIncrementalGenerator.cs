﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StrideSourceGenerator.Core;
using StrideSourceGenerator.ModeInfos.Yaml;
using StrideSourceGenerator.NexAPI;
using System.Collections.Immutable;
using System.Diagnostics;
using StrideSourceGenerator.NexAPI.Analysation.Analyzers;
using Microsoft.CodeAnalysis.CSharp;

namespace StrideSourceGenerator.NexIncremental
{
    [Generator]
    internal class NexIncrementalGenerator : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Debugger.Launch();
            AssignModeInfo assignModeInfo = new AssignModeInfo();
            IncrementalValueProvider<ImmutableArray<ClassInfo>> classProvider = context.SyntaxProvider
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

        private ClassInfo CreateClassInfo(Compilation compilation, TypeDeclarationSyntax classDeclaration, SemanticModel semanticModel)
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

            ClassInfoMemberProcessor classInfoMemberProcessor = new ClassInfoMemberProcessor(memberSelector, compilation);
            classInfoMemberProcessor.PropertyAnalyzers.Add(standardAssignAnalyzer);
            classInfoMemberProcessor.FieldAnalyzers.Add(standardFieldAssignAnalyzer);
            var members = classInfoMemberProcessor.Process(type);
            return ClassInfo.CreateFrom(type, members);
        }

        private static void Generate(
          SourceProductionContext ctx,
          ImmutableArray<ClassInfo> myCustomObjects)
        {
            foreach (ClassInfo obj in myCustomObjects)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                Generates(ctx, obj);
            }
        }
        private static SourceCreator SourceCreator = new SourceCreator();
        private static void Generates(SourceProductionContext ctx, ClassInfo info)
        {
            if (info is null)
                return;

            ctx.AddSource(info.GeneratorName + ".g.cs", SourceCreator.Create(ctx, info));
        }
    }
}