using System;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
internal interface ISerializationReader :  ISerializationStream
{
    bool IsNull();
}
