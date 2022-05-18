using HADotNet.Core.WebSocket.Exceptions;

namespace HADotNet.Core.WebSocket;

/// <summary>
/// A simple client to communicate against the WebSocket API
/// </summary>
public interface IHaWebSocketClient : IDisposable
{
    /// <summary>
    /// Default Timeout for requests
    /// </summary>
    TimeSpan RequestTimeOut { get; set; }

    /// <summary>
    /// Provides a CancellationToken that cancels after
    /// the <see cref="RequestTimeOut"/>
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Unsubscribes the listener from the given message id
    /// </summary>
    /// <param name="messageId">Message Id of the subscription</param>
    void Unsubscribe(int messageId);

    /// <summary>
    /// Subscribes a handler to messages to the Message Id
    /// </summary>
    /// <param name="messageId">Message Id to watch</param>
    /// <param name="handler">Callback handler</param>
    /// <param name="oneTime">Is this a one time command or should it trigger 
    /// on each message?
    /// </param>
    void Subscribe(int messageId, Action<HaMessage> handler, bool oneTime);

    /// <summary>
    /// Sends a Message and subscribes to it's responses
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="callback">Callback handler</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <param name="oneTime">
    /// Is this a one time command or should it trigger 
    /// on each message?</param>
    /// <returns>Message Id</returns>
    Task<int> SendAndSubscribeAsync(HaMessage message, Action<HaMessage> callback,
        CancellationToken cancellationToken, bool oneTime = true);

    /// <summary>
    /// Sends a message over the websocket
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>Message Id</returns>
    Task<int> SendAsync(HaMessage message, CancellationToken token);

    /// <summary>
    /// Connects to the Ha Instance
    /// </summary>
    /// <param name="token">Cancellation Token</param>
    /// <returns>Auth Response</returns>
    /// <exception cref="AuthExeption">
    ///  There was a auth error. Mostly wrong token
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The client receives a unparsable response. Make sure that this is a
    /// Home Assistant Server
    /// </exception>
    /// <exception cref="Exception">
    /// The client receives a message, that it shouldn't receive
    /// </exception>
    Task<AuthOkResponse> ConnectAsync(CancellationToken token);

    /// <summary>
    /// Starts the Listener. On this command, all subscriptions are disposed
    /// </summary>
    void StartListener();

    /// <summary>
    /// Stops the Listener.
    /// </summary>
    void StopListener();

    /// <summary>
    /// Resumes the current listener. No subscription will disposed
    /// </summary>
    void ResumeListener();

    /// <summary>
    /// Triggers when the client connects
    /// </summary>
    event Action OnConnect;

    /// <summary>
    /// Triggers when the client disconnects
    /// </summary>
    event Action OnDisconnect;
}