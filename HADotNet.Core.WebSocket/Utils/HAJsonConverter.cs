using System.Reflection;
using HADotNet.Core.WebSocket.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HADotNet.Core.WebSocket.Utils;

/// <summary>
/// Converts the Json with a map of message types
/// /// </summary>
public class HAJsonConverter : Newtonsoft.Json.JsonConverter
{
    private readonly IReadOnlyDictionary<string, Type> _typeMap;

    public HAJsonConverter(IEnumerable<Assembly> assemblies)
    {
        _typeMap = TypeMapBuilder.BuildTypeMap(assemblies);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableTo(typeof(Message));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var typeName = (string)jObject.Property("type");

        if (!_typeMap.ContainsKey(typeName))
        {
            throw new Exception($"Message Type ${typeName} is not implemented");
        }

        var type = _typeMap[typeName];
        return jObject.ToObject(type);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}