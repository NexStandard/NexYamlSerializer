using NexYamlSourceGenerator.NexAPI;
using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class AbstractRegister : ITemplate
    {
        public string Create(ClassPackage package)
        {
            StringBuilder sb = new();
            foreach (string @abstract in package.ClassInfo.AllAbstracts)
            {
                sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterAbstractClass, "this", @abstract));
            }
            return sb.ToString();
        }
    }
}