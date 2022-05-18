namespace HADotNet.Core.WebSocket.Utils;

/// <summary>
/// Helper to implement a request / response message flow
/// </summary>
/// <typeparam name="TMessage">Type of the response</typeparam>
public class RequestHandler<TMessage> where TMessage : HaMessage
{
    private readonly IHaWebSocketClient _client;
    private readonly TaskCompletionSource<TMessage> _taskCompletion;

    public RequestHandler(IHaWebSocketClient client)
    {
        _taskCompletion = new TaskCompletionSource<TMessage>();
        _client = client;
    }

    public Task<TMessage> Task => _taskCompletion.Task;

    public async Task SendMessage<TRequest>(TRequest request,
        CancellationToken cancellationToken
    )
        where TRequest : HaMessage
    {
        await _client.SendAndSubscribeAsync(
            request,
            HandleResponse,
            cancellationToken
        );

        cancellationToken.Register(CancelRequest);
    }

    private void CancelRequest()
    {
        _taskCompletion.SetException(new TimeoutException());
    }

    private void HandleResponse(HaMessage message)
    {
        var response = (TMessage)message;
        _taskCompletion.SetResult(response);
    }

}