namespace HADotNet.Core.WebSocket.Exceptions;

/// <summary>
/// There is no Listener active.
/// </summary>
[System.Serializable]
public class ListenerInactiveException : System.Exception
{
    public ListenerInactiveException() { }
    public ListenerInactiveException(string message) : base(message) { }
    public ListenerInactiveException(string message, System.Exception inner) : base(message, inner) { }
    protected ListenerInactiveException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}