using NexYamlTest.SimpleClasses;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.Collections;

[DataContract]
public class NullDictionary
{
    public Dictionary<int, int>? dict = null;
}
[DataContract]
internal class Data1 : IDInterface
{
    public int Id { get; set; }
}
[DataContract]
internal class Data2 : IDInterface
{
    public int Id { get; set; }
}
[DataContract]
internal class C
{
    public IEnumerable<int>? Foo { get; set; }
}
[DataContract]
internal class CollectionInterfaces
{
    public ICollection<IDInterface> Collection = [];
    public IReadOnlyCollection<IDInterface> ReadOnlyCollection = new List<IDInterface>();
    public IReadOnlyList<IDInterface> ReadonlyList = [];
    public IList<IDInterface> List = [];
    public IEnumerable<IDInterface> Enumerable = [];
    public IDictionary<int, IDInterface> Dictionary = new Dictionary<int, IDInterface>();
    public IReadOnlyDictionary<IDInterface, IDInterface> ReadonlyDictioanry = new Dictionary<IDInterface, IDInterface>();
}
