namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// Valid Authentication
/// </summary>
[MessageType("auth_ok")]
public class AuthOkResponse : Message
{
    /// <summary>
    /// Home Assistant Version
    /// </summary>
    public string HaVersion { get; set; }
}