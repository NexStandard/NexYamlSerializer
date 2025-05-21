using Microsoft.CodeAnalysis;
using NexYaml.SourceGenerator.Core;
using NexYaml.SourceGenerator.MemberApi.UniversalAnalyzers;

namespace NexYaml.SourceGenerator.MemberApi.Data;

internal record DataMemberContext
{
    private DataMemberContext() { }

    private static DataMemberContext Empty { get; } = new DataMemberContext() { State = DataMemberContextState.Excluded };
    internal static DataMemberContext Create(ISymbol symbol, ReferencePackage references)
    {
        var context = new DataMemberContext();
        if (symbol.TryGetAttribute(references.DataMemberIgnoreAttribute, out _))
        {
            return Empty;
        }
        if (symbol.TryGetAttribute(references.DataMemberAttribute, out var attributeData1))
        {
            context.State = DataMemberContextState.Included;
            if (attributeData1 is { AttributeConstructor.Parameters: [.., { Name: "mode" }], ConstructorArguments: [.., { Value: int mode }] })
            {
                context.Mode = (MemberMode)mode;
                if (context.Mode == MemberMode.Never)
                {
                    context.State = DataMemberContextState.Excluded;
                }

                if (context.Mode == MemberMode.Content)
                {
                    context.State = DataMemberContextState.Included;
                }
            }
            else
            {
                context.Mode = MemberMode.Assign;
            }
            // TODO: Order Mode
            context.Order = -0;
            return context;
        }

        context.State = DataMemberContextState.Weak;
        context.Mode = MemberMode.Assign;
        context.Order = 0;
        return context;
    }
    public DataMemberContextState State { get; private set; }
    public MemberMode Mode { get; private set; }
    public int Order { get; private set; }
}
