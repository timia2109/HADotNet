using HADotNet.Core.WebSocket;
using HADotNet.Core.WebSocket.Clients;
using Newtonsoft.Json;

var instance = new Uri(Environment.GetEnvironmentVariable("HADotNet_Tests_Instance"));
var apiKey = Environment.GetEnvironmentVariable("HADotNet_Tests_ApiKey");

void Report(object o)
{
    Console.WriteLine(JsonConvert.SerializeObject(o));
}

void ReportM(string message, object o)
{
    Console.WriteLine(message + " => " + JsonConvert.SerializeObject(o));
}

var clientBuilder = new WebSocketClientBuilder();
clientBuilder.InstanceUri = instance;
clientBuilder.Token = apiKey;
var _client = clientBuilder.Client;

Console.WriteLine("Connecting to WS");
var cts = new CancellationTokenSource();
cts.CancelAfter(20000);
var response = await _client.ConnectAsync(cts.Token);
ReportM("Connected", response);

const string eventName = "testEvent";
_client.StartListener();

var eventClient = new EventsClient(_client);
var subResult = await eventClient.SubscribeEvent(
    (m) => ReportM("Receive Event", m),
    eventName
);

ReportM("Subscribe Result", subResult);

var result = await eventClient.FireEvent(eventName,
    new Dictionary<string, object>());

ReportM("Fire Result", result);
Thread.Sleep(10000);
_client.StopListener();