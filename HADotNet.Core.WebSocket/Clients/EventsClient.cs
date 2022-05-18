using HADotNet.Core.Models;

namespace HADotNet.Core.WebSocket.Clients;

/// <summary>
/// Client for communicating with Events
/// </summary>
public class EventsClient : AHaClient
{

    public EventsClient(IHaWebSocketClient client) : base(client)
    { }

    public async Task<SuccessResult> SubscribeEvent(
        Action<EventMessage> eventDelegate,
        string eventType = null
    )
    {
        var subscription = new EventSubscription
        {
            EventType = eventType
        };

        var result = await SendAndReceiveAsync<SuccessResult>(subscription,
            Client.CancellationToken);

        if (result.Success)
        {
            Client.Subscribe(result.Id,
                CreateSubscriptionHelper(eventDelegate),
                false);
        }
        return result;
    }

    public Task<SuccessResult> FireEvent(string eventType,
        Dictionary<string, object> eventData)
    {
        return FireEvent(new FireEventMessage
        {
            EventData = eventData,
            EventType = eventType
        });
    }

    public Task<SuccessResult> FireEvent(
        FireEventMessage fireEventMessage)
    {
        return SendAndReceiveAsync<SuccessResult>(
            fireEventMessage, Client.CancellationToken
        );
    }

}