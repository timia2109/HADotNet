using System.Collections.Concurrent;

namespace HADotNet.Core.WebSocket;

/// <summary>
/// Simple Message Listener
/// </summary>
internal class MessageListener : IDisposable
{
    private readonly ConcurrentDictionary<int, Subscription> _callbacks = new();

    private Task _messageLoop;
    private Func<CancellationToken, Task<HaMessage>> _readDelegate;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Constructs a new <see cref="MessageListener"/>
    /// </summary>
    /// <param name="readDelegate">A Delegate to read from the websocket</param>
    public MessageListener(Func<CancellationToken, Task<HaMessage>> readDelegate)
    {
        _readDelegate = readDelegate;
    }

    /// <summary>
    /// Stops the reading loop. New Messages will not more handled
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    /// <summary>
    /// Starts the reading loop. It will start a new <see cref="Task"/> to 
    /// listen to incomming websocket messages
    /// </summary>
    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _messageLoop = MessageLoop();
    }

    /// <summary>
    /// Handles messages in a loop
    /// </summary>
    /// <returns>Task</returns>
    private async Task MessageLoop()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var message = await _readDelegate(
                _cancellationTokenSource.Token
            );
            // Disable Warning (fire & forget)
            var _ = Task.Run(() => HandleMessage(message));
        }
    }

    /// <summary>
    /// Subscribes to a message id. If a message with the specified message id
    /// is received, the callback gets invoked
    /// </summary>
    /// <param name="messageId">Message Id to listen</param>
    /// <param name="callback">Callback delegate</param>
    /// <param name="oneTime">Should this callback only used one time?</param>
    public void Subscribe(
        int messageId,
        Action<HaMessage> callback,
        bool oneTime
    )
    {
        var subscription = new Subscription(callback, oneTime);
        _callbacks.TryAdd(messageId, subscription);
    }

    /// <summary>
    /// Deletes the subscription
    /// </summary>
    /// <param name="messageId">Affected Message Id</param>
    public void Unsubscribe(int messageId)
    {
        _callbacks.Remove(messageId, out var _);
    }

    /// <summary>
    /// Handles a message, which was received from the socket
    /// </summary>
    /// <param name="message">Message</param>
    private void HandleMessage(HaMessage message)
    {
        var messageId = message.Id;
        // Are there active Listener?
        if (!_callbacks.ContainsKey(messageId))
            return;

        var subscription = _callbacks[messageId];

        // Drop if this is a one time subscription
        if (subscription.OneTime)
        {
            Unsubscribe(messageId);
        }

        subscription.Callback(message);
    }

    public void Dispose()
    {
        Stop();
    }

    /// <summary>
    /// Wrapper for a subscription
    /// </summary>
    /// <param name="Callback">Callback delegate</param>
    /// <param name="OneTime">Is this a one time callback?</param>
    /// <returns>Subscription</returns>
    private record Subscription(
       Action<HaMessage> Callback,
       bool OneTime
   );
}