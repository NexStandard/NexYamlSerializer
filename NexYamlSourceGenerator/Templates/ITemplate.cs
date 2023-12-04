using StrideSourceGenerator.NexAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideSourceGenerator.Templates
{
    internal interface ITemplate
    {
        public string Create(ClassInfo info);
    }
}