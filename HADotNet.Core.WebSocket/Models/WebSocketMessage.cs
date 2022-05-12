using Newtonsoft.Json;

namespace HADotNet.Core.WebSocket.Models;

public abstract class WebSocketMessage
{
    public abstract string Type { get; }

    public int Id { get; set; }
}