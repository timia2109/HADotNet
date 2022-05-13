namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// Basic Type for a Home Assistant Message
/// </summary>
public abstract class HaMessage : Message
{
    /// <summary>
    /// Id of this message
    /// </summary>
    public int Id { get; set; }
}