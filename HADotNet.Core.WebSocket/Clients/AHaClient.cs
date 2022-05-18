namespace HADotNet.Core.WebSocket.Clients;

/// <summary>
/// Base Class for a Client
/// </summary>
public abstract class AHaClient
{
    protected IHaWebSocketClient Client { get; }

    public AHaClient(IHaWebSocketClient client)
    {
        Client = client;
    }

    /// <summary>
    /// Sends a message and wait for its response
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <typeparam name="TResult">Response Type</typeparam>
    /// <returns>Result of this request</returns>
    protected async Task<TResult> SendAndReceiveAsync<TResult>(
        HaMessage request, CancellationToken cancellationToken
    ) where TResult : HaMessage
    {
        var requestHandler = new RequestHandler<TResult>(Client);
        await requestHandler.SendMessage(request, cancellationToken);

        var result = await requestHandler.Task;
        return result;
    }

    protected Action<HaMessage> CreateSubscriptionHelper<TResult>(
        Action<TResult> callback
    ) where TResult : HaMessage {
        return new SubscriptionHelper<TResult>(callback).OnMessage;
    }

    private class SubscriptionHelper<TResult> where TResult : HaMessage
    {
        private readonly Action<TResult> _callback;

        public SubscriptionHelper(Action<TResult> callback)
        {
            _callback = callback;
        }

        public void OnMessage(HaMessage message)
        {
            var eventMessage = (TResult)message;
            _callback(eventMessage);
        }
    }
}