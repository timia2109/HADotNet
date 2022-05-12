namespace HADotNet.Core.WebSocket.Models;

public class AuthRequest
{
    public string Type => "auth";

    public string AccessToken { get; set; }
}