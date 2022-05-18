namespace HADotNet.Core.WebSocket.Models;

[MessageType("fire_event")]
public class FireEventMessage : HaMessage
{

    public string EventType { get; set; }

    public Dictionary<string, object> EventData { get; set; }

}