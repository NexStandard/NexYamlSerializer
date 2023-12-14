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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Constants.SerializerRegistry}.RegisterTag($\"{package.ClassInfo.NameSpace}.{package.ClassInfo.TypeName},{{AssemblyName}}\",typeof({package.ClassInfo.ShortDefinition}));");
            if(package.ClassInfo.IsGeneric)
            {
                sb.AppendLine($"{Constants.SerializerRegistry}.RegisterGenericFormatter(typeof({package.ClassInfo.ShortDefinition}),typeof({package.ClassInfo.GeneratorName + package.ClassInfo.TypeParameterArgumentsShort}));");
                sb.AppendLine($"{Constants.SerializerRegistry}.RegisterFormatter(typeof({package.ClassInfo.ShortDefinition}));");
                return sb.ToString();
            }
            return sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterFormatter, $"new {package.ClassInfo.NameSpace}.{package.ClassInfo.GeneratorName}()")).ToString();
        }
    }
}