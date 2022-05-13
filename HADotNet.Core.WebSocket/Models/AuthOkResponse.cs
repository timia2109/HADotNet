namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// Valid Authentication
/// </summary>
[MessageType("auth_ok")]
public class AuthOkResponse
{
    /// <summary>
    /// Home Assistant Version
    /// </summary>
    public string HaVersion { get; set; }
}