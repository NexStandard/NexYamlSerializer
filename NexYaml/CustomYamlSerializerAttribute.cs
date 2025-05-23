using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CustomYamlSerializerAttribute : Attribute
{
    /// <summary>
    /// the type to skip during source generation, while still source generating the <see cref="IYamlSerializerFactory"/>
    /// </summary>
    public required Type TargetType { get; init; }
    /// <summary>
    /// overrides the Tag used for the registration
    /// </summary>
    public string? Tag { get; init; }
}
