using System.Net.WebSockets;
using HADotNet.Core.WebSocket.Models;
using HADotNet.Core.WebSocket.Utils;

namespace HADotNet.Core.WebSocket;

public class BasicWebSocketClient : IDisposable
{

    private readonly Uri _instanceUri;
    private readonly string _token;
    private readonly IdCounter _idCounter;
    private readonly ClientWebSocket _webSocket;
    private readonly JsonConverter _json;

    public BasicWebSocketClient(Uri instanceUri, string token)
    {
        _instanceUri = instanceUri;
        _token = token;

        _webSocket = new ClientWebSocket();
        _idCounter = new IdCounter();
        _json = new JsonConverter();
    }

    private AuthRequest AuthRequest => new AuthRequest
    {
        AccessToken = _token
    };

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _webSocket.ConnectAsync(_instanceUri, cancellationToken);
        await _webSocket.SendAsync(
            _json.Serialize(AuthRequest), WebSocketMessageType.Text,
            true, cancellationToken
        );


    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}