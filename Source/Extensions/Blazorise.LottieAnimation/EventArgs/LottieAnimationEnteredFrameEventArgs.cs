namespace Blazorise.LottieAnimation;

/// <summary>
/// Event arguments for the EnterFrame event
/// </summary>
public record LottieAnimationEnteredFrameEventArgs
{
    /// <summary>
    /// Gets the current frame time.
    /// </summary>
    public double CurrentTime { get; init; }

    /// <summary>
    /// Gets the total time for the entire animation.
    /// </summary>
    public double TotalTime { get; init; }

    /// <summary>
    /// Gets the direction of animation playback.
    /// </summary>
    public LottieAnimationDirection Direction { get; init; }
}