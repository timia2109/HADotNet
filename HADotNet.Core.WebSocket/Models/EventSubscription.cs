namespace HADotNet.Core.WebSocket.Models;

[MessageType("subscribe_events")]
public class EventSubscription : HaMessage
{
    /// <summary>
    /// Name of the Event (optional)
    /// </summary>
    public string EventType { get; set; }
}