namespace HADotNet.Core.WebSocket.Utils;

/// <summary>
/// A simple, Threadsafe Counter
/// </summary>
public class IdCounter
{
    /// <summary>
    /// Current message id
    /// </summary>
    private int _messageId;

    /// <summary>
    /// Creates the counter
    /// </summary>
    public IdCounter()
    {
        _messageId = 0;
    }

    /// <summary>
    /// Increases the counter
    /// </summary>
    /// <returns>Current Value</returns>
    public int Increase()
    {
        return Interlocked.Increment(ref _messageId);
    }

    /// <summary>
    /// Gets the current value (without increment)
    /// </summary>
    public int CurrentValue => _messageId;

    /// <summary>
    /// Gets the value after a increment
    /// </summary>
    public int Value => Increase();
}