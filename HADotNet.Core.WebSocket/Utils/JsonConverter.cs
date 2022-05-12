using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HADotNet.Core.WebSocket.Utils;

public class JsonConverter
{

    private readonly JsonSerializer _serializer;

    public JsonConverter()
    {
        _serializer = new JsonSerializer
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
    }

    public TValue Deserialize<TValue>(ReadOnlyMemory<byte> readOnlyMemory)
    {
        using var stream = new MemoryStream(readOnlyMemory.ToArray());
        stream.Position = 0;
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);

        return _serializer.Deserialize<TValue>(jsonReader);
    }

    public ReadOnlyMemory<byte> Serialize<TValue>(TValue data)
    {
        using var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(streamWriter);

        _serializer.Serialize(jsonWriter, data);

        stream.Position = 0;
        return new ReadOnlyMemory<byte>(stream.ToArray());
    }

}