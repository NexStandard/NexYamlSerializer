using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;

namespace NexYaml.XParser
{
    public class IdentifiableResolver
    {
        private readonly Dictionary<Guid, (TaskCompletionSource<object>? tcs, object? result)> _identifiables = [];
        public void RegisterIdentifiable(Guid guid, IIdentifiable identifiable)
        {
            ref var field = ref CollectionsMarshal.GetValueRefOrAddDefault(_identifiables, guid, out _);
            if (field.tcs is not null) // Something is waiting, notify it;
                field.tcs.SetResult(identifiable);
            else // Nothing was waiting on this one, set the field
                field.result = identifiable;
        }
        public async ValueTask<T> AsyncGetRef<T>(Guid guid)
        {
            (TaskCompletionSource<object>? tcs, object? result) tcs;
            if (_identifiables.TryGetValue(guid, out var value))
            {
                if (value.result is not null)
                {
                    return (T)value.result;
                }
                tcs = value;
            }
            else
            {
                tcs = new()
                {
                    tcs = new TaskCompletionSource<object>()
                };
                _identifiables.Add(guid, tcs);
            }
            return (T)(await tcs.tcs!.Task);
        }
    }
}
