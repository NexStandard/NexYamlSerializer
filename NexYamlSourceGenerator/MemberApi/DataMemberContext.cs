using Microsoft.CodeAnalysis;
using NexYamlSourceGenerator.Core;
using NexYamlSourceGenerator.MemberApi.UniversalAnalyzers;
using System.Runtime.Serialization;

namespace NexYamlSourceGenerator.MemberApi;

internal record DataMemberContext
{
    private DataMemberContext() { }
    static DataMemberContext Empty { get; } = new DataMemberContext() { State = DataMemberContextState.Excluded };
    internal static DataMemberContext Create(ISymbol symbol, ReferencePackage references)
    {
        var context = new DataMemberContext();

        if (symbol.TryGetAttribute(references.DataMemberIgnoreAttribute, out var attributeData))
        {
            return Empty;
        }
        else
        {
            if(symbol.TryGetAttribute(references.DataMemberAttribute, out var attributeData1))
            {
                context.State = DataMemberContextState.Included;
                // TODO: Assign, Content Mode
                context.Mode = MemberMode.Assign;
                // TODO: Order Mode
                context.Order = 0;
            }
            else
            {
                context.State = DataMemberContextState.Weak;
                context.Mode = MemberMode.Assign;
                context.Order = 0;
            }
        }
        return context;
    }
    public DataMemberContextState State { get; private set; }
    public MemberMode Mode { get; set; }
    public int Order { get; set; }
}
internal enum DataMemberContextState
{
    Included, // Field was Datamembered
    Weak,     // Field wasnt Datamemberd but also not excluded
    Excluded  // DataMemberIgnored
}