using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HADotNet.Core.WebSocket;
using HADotNet.Core.WebSocket.Clients;
using HADotNet.Core.WebSocket.Models;
using NUnit.Framework;

namespace HADotNet.Core.Tests
{

    public class WebsocketEventTests
    {

        private Uri Instance { get; set; }
        private string ApiKey { get; set; }
        private BasicWebSocketClient _client;

        [SetUp]
        public async Task SetUp()
        {
            Instance = new Uri(Environment.GetEnvironmentVariable("HADotNet:Tests:Instance"));
            ApiKey = Environment.GetEnvironmentVariable("HADotNet:Tests:ApiKey");

            var clientBuilder = new WebSocketClientBuilder();
            clientBuilder.InstanceUri = Instance;
            clientBuilder.Token = ApiKey;
            _client = clientBuilder.Client;

            Console.WriteLine("Connecting to WS");
            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            var response = await _client.ConnectAsync(cts.Token);
            Assert.IsNotEmpty(response.HaVersion);
            Console.WriteLine("Connected to WS");
        }

        [Test]
        public async Task EventTest()
        {
            const string eventName = "testEvent";
            _client.StartListener();

            var eventListener = new EventListener();
            var eventClient = new EventsClient(_client);
            var subResult = await eventClient.SubscribeEvent(
                eventListener.OnEventMessage,
                eventName
            );

            Assert.IsTrue(subResult.Success);
            Console.WriteLine("Subscribed");

            var result = await eventClient.FireEvent(eventName,
                new Dictionary<string, object>());

            Assert.IsTrue(result.Success);
            Console.WriteLine("Fired!");

            var trigger = await eventListener.Task;
        }

        private class EventListener
        {
            private readonly TaskCompletionSource<EventMessage> _compleation
                = new();

            public void OnEventMessage(EventMessage eventMessage)
            {
                _compleation.SetResult(eventMessage);
            }

            public Task<EventMessage> Task => _compleation.Task;
        }

    }

}