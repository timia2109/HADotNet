namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// A login request
/// </summary>
[MessageType("auth")]
public class AuthRequest : Message
{
    /// <summary>
    /// Access Token
    /// </summary>
    public string AccessToken { get; set; }
}