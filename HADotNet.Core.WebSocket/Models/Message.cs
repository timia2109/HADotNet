using System.Reflection;
using HADotNet.Core.WebSocket.Utils;

namespace HADotNet.Core.WebSocket.Models;

public abstract class Message
{
    /// <summary>
    /// Type of this message 
    /// </summary>
    public string Type => GetType().GetCustomAttribute<MessageTypeAttribute>()
        .TypeValue;
}