using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class AbstractRegister : ITemplate
    {
        public string Create(ClassInfo info)
        {
            StringBuilder sb = new();
            //foreach (string @abstract in info.AllAbstracts)
            {
                //  sb.AppendLine(Constants.SerializerRegistry + string.Format(Constants.RegisterAbstractClass, "this", @abstract));
            }
            return sb.ToString();
        }
    }
}