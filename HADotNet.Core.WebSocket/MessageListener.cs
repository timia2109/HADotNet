using System.Collections.Concurrent;

namespace HADotNet.Core.WebSocket;

internal class MessageListener
{
    private readonly ConcurrentDictionary<int, Subscription> _callbacks = new();

    private Task _messageLoop;
    private bool _started;
    private Func<CancellationToken, Task<HaMessage>> _readDelegate;
    private CancellationTokenSource _cancellationTokenSource;

    public MessageListener(Func<CancellationToken, Task<HaMessage>> readDelegate)
    {
        _readDelegate = readDelegate;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Stop()
    {
        _started = false;
        _cancellationTokenSource.Cancel();
    }

    public void Start()
    {
        _started = true;
        _messageLoop = MessageLoop();
    }

    public async Task MessageLoop()
    {
        while (_started)
        {
            var message = await _readDelegate(
                _cancellationTokenSource.Token
            );
            Task.Run(() => HandleMessage(message));
        }
    }

    public void Subscribe(
        int messageId,
        Action<HaMessage> callback,
        bool oneTime
    )
    {
        var subscription = new Subscription(callback, oneTime);
        _callbacks.TryAdd(messageId, subscription);
    }

    private void HandleMessage(HaMessage message)
    {
        // TODO: Handle Message
    }

    private record Subscription(
       Action<HaMessage> Callback,
       bool OneTime
   );
}