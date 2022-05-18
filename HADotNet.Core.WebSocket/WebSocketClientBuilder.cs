using System.Reflection;

namespace HADotNet.Core.WebSocket;

/// <summary>
/// Simple Builder for the WebSocket Client
/// </summary>
public class WebSocketClientBuilder
{
    /// <summary>
    /// List of <see cref="Assembly"/> which get scanned for classes with the 
    /// Attribute of <see cref="MessageTypeAttribute"/>
    /// </summary>
    public List<Assembly> Assemblies { get; }

    /// <summary>
    /// Uri of the Instance
    /// </summary>
    public Uri InstanceUri { get; set; }

    /// <summary>
    /// Token for the instance
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets the client
    /// </summary>
    public IHaWebSocketClient Client => Build();

    public WebSocketClientBuilder()
    {
        Assemblies = new List<Assembly>{
            GetType().Assembly
        };
    }

    private HaWebSocketClient Build()
    {
        if (string.IsNullOrEmpty(Token))
            throw new ArgumentNullException(nameof(Token));

        if (InstanceUri == null)
            throw new ArgumentNullException(nameof(InstanceUri));

        if (!Assemblies.Any())
            throw new ArgumentException($"{nameof(Assemblies)} is empty");

        return new HaWebSocketClient(InstanceUri, Token, Assemblies);
    }

}