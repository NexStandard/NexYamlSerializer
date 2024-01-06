using NexYamlSourceGenerator.Core;

namespace NexYamlSourceGenerator.MemberApi.Data;

internal enum DataMemberContextState
{
    /// <summary>
    /// Member has a <see cref="ReferencePackage.DataMemberAttribute"/>
    /// </summary>
    Included,
    /// <summary>
    /// Member doesn't have a <see cref="ReferencePackage.DataMemberAttribute"/> but also not a <see cref="ReferencePackage.DataMemberIgnoreAttribute"/>
    /// </summary>
    Weak,
    /// <summary>
    /// Member has a <see cref="ReferencePackage.DataMemberIgnoreAttribute"/>
    /// or // TODO Never mode
    /// </summary>
    Excluded
}