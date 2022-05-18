namespace HADotNet.Core.WebSocket.Utils;

/// <summary>
/// Helper to implement a request / response message flow
/// </summary>
/// <typeparam name="TMessage">Type of the response</typeparam>
public class RequestHandler<TMessage> where TMessage : HaMessage
{
    private readonly IHaWebSocketClient _client;
    private readonly TaskCompletionSource<TMessage> _compleation;

    public RequestHandler(IHaWebSocketClient client)
    {
        _compleation = new TaskCompletionSource<TMessage>();
        _client = client;
    }

    public Task<TMessage> Task => _compleation.Task;

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
        _compleation.SetException(new TimeoutException());
    }

    private void HandleResponse(HaMessage message)
    {
        var response = (TMessage)message;
        _compleation.SetResult(response);
    }

}