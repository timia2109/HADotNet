using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HADotNet.Core.WebSocket.Utils;

public class JsonConverter
{

    private readonly JsonSerializer _serializer;

    public JsonConverter(IEnumerable<Assembly> assemblies)
    {
        _serializer = new JsonSerializer
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        _serializer.Converters.Add(new HAJsonConverter(assemblies));
    }

    public TValue Deserialize<TValue>(MemoryStream stream)
    {
        stream.Position = 0;
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);

        return _serializer.Deserialize<TValue>(jsonReader);
    }

    public ArraySegment<byte> Serialize<TValue>(TValue data)
    {
        using var stream = new MemoryStream();
        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
        using (var jsonWriter = new JsonTextWriter(streamWriter))
        {
            _serializer.Serialize(jsonWriter, data);
        }

        var streamArray = stream.ToArray();
        return new ArraySegment<byte>(streamArray, 3, streamArray.Length - 3);
    }

}