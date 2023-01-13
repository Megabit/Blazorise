namespace Blazorise.LottieAnimation;

/// <summary>
/// Event arguments passed when the animation has loaded.
/// </summary>
public record LottieAnimationLoadedEventArgs
{
    /// <summary>
    /// Gets the current frame of the animation.
    /// </summary>
    public double CurrentFrame { get; init; }

    /// <summary>
    /// Gets the total frames for the entire animation.
    /// </summary>
    public double TotalFrames { get; init; }
};