namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// Server Request for authentication
/// </summary>
[MessageType("auth_required")]
public class AuthRequestCommand : Message
{
    /// <summary>
    /// Home Assistant Version
    /// </summary>
    public string HaVersion { get; set; }
}