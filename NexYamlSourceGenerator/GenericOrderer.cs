using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexYamlSourceGenerator;
internal class GenericOrderer
{
    public void T(INamedTypeSymbol symbol)
    {
    }
}

interface Test<T,K>
{

}
public class H<T> : Test<int,T>
{
    public static void Instantiate<T>()
    {
        var x = typeof(T).GenericTypeArguments[1];
    }
}
