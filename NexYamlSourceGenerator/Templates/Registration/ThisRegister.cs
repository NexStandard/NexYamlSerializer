using NexYamlSourceGenerator.NexAPI;
using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class ThisRegister : ITemplate
    {

        public string Create(ClassPackage package)
        {
            if(package.ClassInfo.IsGeneric)
            {
                var genericBuilder = new StringBuilder();
                genericBuilder.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericFormatter(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
                genericBuilder.AppendLine($"{Constants.SerializerRegistry}.RegisterFormatter(typeof({package.ClassInfo.ShortDefinition}));");
                return genericBuilder.ToString();
            }
            return Constants.SerializerRegistry + string.Format(Constants.RegisterFormatter, "this");
        }
    }
}