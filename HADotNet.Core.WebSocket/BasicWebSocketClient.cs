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
    private MessageListener _messageListener;

    /// <summary>
    /// Default Timeout
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public CancellationToken CancellationToken
    {
        get
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(RequestTimeout);
            return source.Token;
        }
    }

    internal BasicWebSocketClient(Uri instanceUri,
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

    private async Task<HaMessage> ReadHaMessageAsync(
        CancellationToken cancellationToken)
    {
        var response = await ReadAsync(cancellationToken);
        return (HaMessage)response;
    }

    private async Task<Message> ReadAsync(CancellationToken cancellationToken)
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

        return _json.Deserialize<Message>(messageStream);
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

    public void Unsubscribe(int messageId)
    {
        CheckActiveListener();
        _messageListener.Unsubscribe(messageId);
    }

    public void Subscribe(
        int messageId,
        Action<HaMessage> message,
        bool oneTime
    )
    {
        CheckActiveListener();
        _messageListener.Subscribe(messageId, message, oneTime);
    }

    /// <summary>
    /// Sends a message through the socket and subscribes to it answer
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="callback">The callback</param>
    /// <param name="cancellationToken">Cancel operation</param>
    /// <param name="oneTime">Is this a one time subscription</param>
    /// <returns>Message Id</returns>
    public async Task<int> SendAndSubscribeAsync(
        HaMessage message,
        Action<HaMessage> callback,
        CancellationToken cancellationToken,
        bool oneTime = true
    )
    {
        CheckActiveListener();
        var messageId = _idCounter.Value;
        _messageListener.Subscribe(
            messageId,
            callback,
            oneTime
        );
        await SendAsync(message, messageId, cancellationToken);
        return messageId;
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
    public async Task<AuthOkResponse> ConnectAsync(CancellationToken cancellationToken)
    {
        await _webSocket.ConnectAsync(_instanceUri, cancellationToken);
        await _webSocket.SendAsync(
            _json.Serialize(AuthRequest), WebSocketMessageType.Text,
            true, cancellationToken
        );

        // Wait for auth request
        var response = await ReadAsync(cancellationToken);

        if (response is not AuthRequestCommand)
        {
            throw new Exception($"Wrong API Response {response.GetType().Name}");
        }

        // Send auth
        await SendAsync(AuthRequest, cancellationToken);

        // Wait for result
        var authResponse = await ReadAsync(cancellationToken);

        // Accept auth
        if (authResponse is AuthInvalidResponse air)
        {
            throw new AuthException(air);
        }
        else if (authResponse is AuthOkResponse ok)
        {
            // Everything is great
            return ok;
        }
        else
        {
            throw new Exception(
                $"Type is not allowed at this state {authResponse.GetType().Name}"
            );
        }
    }

    public void StartListener()
    {
        _messageListener = new MessageListener(ReadHaMessageAsync);
    }

    public void StopListener()
    {
        CheckActiveListener();
        _messageListener.Dispose();
    }

    private void CheckActiveListener()
    {
        if (_messageListener == null)
        {
            throw new ListenerInactiveException();
        }
    }

    public void Dispose()
    {
        _messageListener?.Dispose();
        _webSocket.Dispose();
    }


}