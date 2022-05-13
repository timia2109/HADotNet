namespace HADotNet.Core.WebSocket.Models;

/// <summary>
/// Invalid Auth
/// </summary>
[MessageType("auth_invalid")]
public class AuthInvalidResponse : Message
{

    /// <summary>
    /// Message of this rejected auth
    /// </summary>
    public string Message { get; set; }

}