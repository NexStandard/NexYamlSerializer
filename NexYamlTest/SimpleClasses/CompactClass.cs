using Stride.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlTest.SimpleClasses;

[DataContract()]
[DataStyle(DataStyle.Compact)]
internal record class CompactParent
{

}
[DataContract()]
[DataStyle(DataStyle.Compact)]
internal record class CompactClass : CompactParent
{
    public string Name = "Bob";
    public int Id = 1;
}
