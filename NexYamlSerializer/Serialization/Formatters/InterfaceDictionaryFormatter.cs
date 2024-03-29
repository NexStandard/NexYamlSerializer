#nullable enable
using System.Collections.Generic;
using System.Linq;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Audio;
using Stride.Core;

namespace NexVYaml.Serialization;

public class InterfaceDictionaryFormatter<TKey, TValue> : YamlSerializer<IDictionary<TKey, TValue>?>,IYamlFormatter<IDictionary<TKey, TValue>?>
{
    public void Serialize(ref Utf8YamlEmitter emitter, IDictionary<TKey, TValue>? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {

        if (value is null)
        {
            emitter.WriteNull();
            return;
        }
        else
        {
            context.IsRedirected = false;

            IYamlFormatter<TKey> keyFormatter = null;
            IYamlFormatter<TValue> valueFormatter = null;
            if (this.IsPrimitiveType(typeof(TKey)))
            {
                keyFormatter = context.Resolver.GetFormatter<TKey>();
            }
            if (this.IsPrimitiveType(typeof(TValue)))
                valueFormatter = context.Resolver.GetFormatter<TValue>();

            if (keyFormatter == null)
            {
                emitter.BeginSequence();
                if (value.Count > 0)
                {
                    var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
                    foreach (var x in value)
                    {
                        elementFormatter.Serialize(ref emitter, x, context);
                    }
                }
                emitter.EndSequence();
            }
            else if (valueFormatter == null)
            {
                emitter.BeginMapping();
                {
                    foreach (var x in value)
                    {
                        keyFormatter.Serialize(ref emitter, x.Key, context);
                        context.Serialize(ref emitter, x.Value);
                    }
                }
                emitter.EndMapping();
            }
            else
            {
                emitter.BeginMapping();
                {
                    foreach (var x in value)
                    {
                        keyFormatter.Serialize(ref emitter, x.Key, context);
                        valueFormatter.Serialize(ref emitter, x.Value, context);
                    }
                }
                emitter.EndMapping();
            }
        }
    }

    public override IDictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }
        var map = new Dictionary<TKey, TValue>();
        if (this.IsPrimitiveType(typeof(TKey)))
        {
            var keyFormatter = context.Resolver.GetFormatter<TKey>();
            parser.ReadWithVerify(ParseEventType.MappingStart);


            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = context.DeserializeWithAlias(keyFormatter, ref parser);
                var value = context.DeserializeWithAlias<TValue>(ref parser);
                map.Add(key, value);
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return map;
        }
        else
        {
            var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
            var keyValuePairs = context.DeserializeWithAlias(listFormatter, ref parser);

            return keyValuePairs?.ToDictionary() ?? [];
        }
    }

    public override void Serialize(ref IYamlStream stream, IDictionary<TKey, TValue>? value, DataStyle style = DataStyle.Normal)
    {
        /*stream.SerializeContext.IsRedirected = false;

        IYamlFormatter<TKey> keyFormatter = null;
        IYamlFormatter<TValue> valueFormatter = null;
        if (this.IsPrimitiveType(typeof(TKey)))
        {
            keyFormatter = stream.SerializeContext.Resolver.GetFormatter<TKey>();
        }
        if (this.IsPrimitiveType(typeof(TValue)))
            valueFormatter = stream.SerializeContext.Resolver.GetFormatter<TValue>();

        if (keyFormatter == null)
        {
            stream.Emitter.BeginSequence();
            if (value.Count > 0)
            {
                var elementFormatter = new KeyValuePairFormatter<TKey, TValue>();
                foreach (var x in value)
                {
                    elementFormatter.Serialize(ref stream, x, stream.SerializeContext);
                }
            }
            stream.Emitter.EndSequence();
        }
        else if (valueFormatter == null)
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream,x.Key, stream.SerializeContext);
                    stream.Write(x.Value);
                }
            }
            stream.Emitter.EndMapping();
        }
        else
        {
            stream.Emitter.BeginMapping();
            {
                foreach (var x in value)
                {
                    keyFormatter.Serialize(ref stream, x.Key, stream.SerializeContext);
                    valueFormatter.Serialize(ref stream, x.Value, stream.SerializeContext);
                }
            }
            stream.Emitter.EndMapping();
        }*/
    }
}

