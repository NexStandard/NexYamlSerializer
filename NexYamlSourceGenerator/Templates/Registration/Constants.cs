using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class Constants
    {
        public const string SerializerRegistry = "NexYamlSerializerRegistry.Instance";
        public const string RegisterFormatter = ".RegisterFormatter({0});";
        public const string RegisterInterface = ".RegisterInterface({0},typeof({1}));";
        public const string RegisterAbstractClass = ".RegisterAbstractClass({0},typeof({1}));";
    }
}