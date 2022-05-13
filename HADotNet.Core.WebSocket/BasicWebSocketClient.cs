using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reflection;
using HADotNet.Core.WebSocket.Exceptions;

namespace HADotNet.Core.WebSocket;

public class BasicWebSocketClient : IDisposable
{

    private readonly Uri _instanceUri;
    private readonly string _token;
    private readonly IdCounter _idCounter;
    private readonly ClientWebSocket _webSocket;
    private readonly JsonConverter _json;

    public BasicWebSocketClient(Uri instanceUri,
        string token,
        IEnumerable<Assembly> assemblies)
    {
        _instanceUri = instanceUri;
        _token = token;

        _webSocket = new ClientWebSocket();
        _idCounter = new IdCounter();
        _json = new JsonConverter(assemblies);
    }

    private AuthRequest AuthRequest => new AuthRequest
    {
        AccessToken = _token
    };

    private async Task<TType> ReadAsync<TType>(
        CancellationToken cancellationToken) where TType : Message
    {
        var messageStream = new MemoryStream();
        bool complete = false;

        while (!complete)
        {
            var arraySegment = new ArraySegment<byte>();
            var response = await _webSocket
                .ReceiveAsync(arraySegment, cancellationToken);

            messageStream.Write(arraySegment);

            complete = response.EndOfMessage;
        }

        return (TType)_json.Deserialize<Message>(messageStream);
    }

    private async Task SendAsync(Message message,
        CancellationToken cancellationToken)
    {
        await _webSocket.SendAsync(
            _json.Serialize(message),
            WebSocketMessageType.Text,
            true,
            cancellationToken
        );
    }

    public async Task<int> SendAndSubscribeAsync(
        HaMessage message,
        Action<HaMessage> callback,
        CancellationToken cancellationToken,
        bool oneTime = true
    )
    {
        var messageId = _idCounter.Value;
        var subscription = new Subscription(callback, oneTime);

    }

    /// <summary>
    /// Sends a message through the WebSocket
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>Message Id</returns>
    public async Task<int> SendAsync(HaMessage message, CancellationToken token)
    {
        var messageId = _idCounter.Value;
        await SendAsync(message, messageId, token);
        return messageId;
    }

    /// <summary>
    /// Sends a message through the WebSocket
    /// </summary>
    /// <param name="message">Message Object</param>
    /// <param name="messageId">Message Id</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>Message Id</returns>
    private async Task<int> SendAsync(HaMessage message,
        int messageId,
        CancellationToken token)
    {
        message.Id = messageId;

        await SendAsync((Message)message, token);
        return messageId;
    }

    /// <summary>
    /// Connects and authenticates against the instance
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Task</returns>
    /// <exception cref="AuthException">
    /// The Client was unable to authenticate
    /// </exception>
    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await _webSocket.ConnectAsync(_instanceUri, cancellationToken);
        await _webSocket.SendAsync(
            _json.Serialize(AuthRequest), WebSocketMessageType.Text,
            true, cancellationToken
        );

        // Wait for auth request
        var response = await ReadAsync<AuthRequestCommand>(cancellationToken);

        // Send auth
        await SendAsync(AuthRequest, cancellationToken);

        // Wait for result
        var authResponse = await ReadAsync<Message>(cancellationToken);

        // Accept auth
        if (authResponse is AuthInvalidResponse air)
        {
            throw new AuthException(air);
        }
    }

    public void Dispose()
    {
        _webSocket.Dispose();
    }


}