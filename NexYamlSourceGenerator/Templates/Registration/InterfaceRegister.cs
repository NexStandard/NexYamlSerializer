using NexYamlSourceGenerator.NexAPI;
using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class InterfaceRegister : ITemplate
    {
        public string Create(ClassPackage package)
        {
            StringBuilder sb = new();
            foreach (string interfac in package.ClassInfo.AllInterfaces)
            {
                sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterInterface, "this", interfac));
            }
            return sb.ToString();
        }
    }
}