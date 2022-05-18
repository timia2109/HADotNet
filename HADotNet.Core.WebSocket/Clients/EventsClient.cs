using HADotNet.Core.Models;

namespace HADotNet.Core.WebSocket.Clients;

/// <summary>
/// Client for communicating with Events
/// </summary>
public class EventsClient
{

    private readonly BasicWebSocketClient _client;

    public EventsClient(BasicWebSocketClient client)
    {
        _client = client;
    }

    public async Task<SuccessResult> SubscribeEvent(
        Action<EventMessage> eventDelegate,
        string eventType = null
    )
    {
        var subscription = new EventSubscription
        {
            EventType = eventType
        };

        var requestHandler = new RequestHandler<SuccessResult>(_client);
        await requestHandler.SendMessage(subscription,
            _client.CancellationToken);
        var result = await requestHandler.Task;

        if (result.Success)
        {
            var helper = new EventSubscriptionHelper(eventDelegate);
            _client.Subscribe(result.Id, helper.OnMessage, false);
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

    public async Task<SuccessResult> FireEvent(
        FireEventMessage fireEventMessage)
    {
        var requestHandler = new RequestHandler<SuccessResult>(_client);
        await requestHandler.SendMessage(fireEventMessage,
            _client.CancellationToken);
        var result = await requestHandler.Task;

        return result;
    }

    private class EventSubscriptionHelper
    {
        private readonly Action<EventMessage> _callback;

        public EventSubscriptionHelper(Action<EventMessage> callback)
        {
            _callback = callback;
        }

        public void OnMessage(HaMessage message)
        {
            var eventMessage = (EventMessage)message;
            _callback(eventMessage);
        }
    }
}