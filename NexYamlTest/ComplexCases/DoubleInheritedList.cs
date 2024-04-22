using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexYamlTest.ComplexCases;

[DataContract]
internal struct TempList<T,K>
{ }
[DataContract]
internal class DoubleInheritedList : ListWithOnEditCallback<int>
{
}
[DataContract(Inherited = true)]
public class ListWithOnEditCallback<T> : List<T>
{
    public Action? OnEditCallBack { get; internal set; }

    public new void Add(T item)
    {
        base.Add(item);
        OnEditCallBack?.Invoke();
    }
    public new void Remove(T item)
    {
        base.Remove(item);
        OnEditCallBack?.Invoke();
    }
    public new void RemoveAll(Predicate<T> match)
    {
        base.RemoveAll(match);
        OnEditCallBack?.Invoke();
    }
    public new void RemoveAt(int index)
    {
        base.RemoveAt(index);
        OnEditCallBack?.Invoke();
    }
    public new void RemoveRange(int index, int count)
    {
        base.RemoveRange(index, count);
        OnEditCallBack?.Invoke();
    }
    public new void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        OnEditCallBack?.Invoke();
    }
    public new void Clear()
    {
        base.Clear();
        OnEditCallBack?.Invoke();
    }
}