namespace HADotNet.Core.WebSocket.Models;

[MessageType("event")]
public class EventMessage : HaMessage
{
    public EventData Event { get; set; }
}

public class EventData
{
    public Dictionary<string, object> Data { get; set; }
}