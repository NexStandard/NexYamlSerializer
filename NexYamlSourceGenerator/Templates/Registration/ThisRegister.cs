using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates.Registration
{
    internal class ThisRegister : ITemplate
    {

        public string Create(ClassInfo info)
        {
            return Constants.SerializerRegistry + string.Format(Constants.RegisterFormatter, "this");
        }
    }
}