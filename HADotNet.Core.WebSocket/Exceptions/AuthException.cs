namespace HADotNet.Core.WebSocket.Exceptions;

/// <summary>
/// Signals, that the client could not autenticate against the server
/// </summary>
[System.Serializable]
public class AuthException : System.Exception
{
    public AuthException(AuthInvalidResponse authInvalidResponse) :
        base($"Auth Invalid: {authInvalidResponse.Message}")
    { }


}