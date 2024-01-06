using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator.Templates;

internal static class Constants
{
    public const string SerializerRegistry = "\t\tresolver";
    public const string RegisterFormatter = ".RegisterFormatter({0});";
    public const string RegisterInterface = ".Register({0},typeof({1}));";
    public const string RegisterAbstractClass = ".Register({0},typeof({1}));";
}