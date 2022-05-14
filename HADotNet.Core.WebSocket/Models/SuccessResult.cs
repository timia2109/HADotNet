namespace HADotNet.Core.Models;

[MessageType("result")]
public class SuccessResult : HaMessage
{
    /// <summary>
    /// Was this request successful?
    /// </summary>
    public bool Success { get; set; }

    // TODO: Add result parameter 
}