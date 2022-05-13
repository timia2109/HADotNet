namespace HADotNet.Core.WebSocket.Utils;

/// <summary>
/// Defines the Message Type for a <see cref="Message"/>
/// </summary>
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class MessageTypeAttribute : System.Attribute
{
    public string TypeValue { get; }

    public MessageTypeAttribute(string typeValue)
    {
        TypeValue = typeValue;
    }
}