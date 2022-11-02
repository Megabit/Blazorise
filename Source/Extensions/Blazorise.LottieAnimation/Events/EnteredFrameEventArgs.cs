namespace Blazorise.LottieAnimation;

/// <summary>
/// Event arguments for the EnterFrame event
/// </summary>
public record EnteredFrameEventArgs
{
    /// <summary>
    /// Current frame time
    /// </summary>
    public double CurrentTime { get; init; }

    /// <summary>
    /// Total time for the entire animation
    /// </summary>
    public double TotalTime { get; init; }

    /// <summary>
    /// Direction of animation playback
    /// </summary>
    public AnimationDirection Direction { get; init; }
}