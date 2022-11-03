namespace Blazorise.LottieAnimation;

public record LoadedEventArgs
{
    /// <summary>
    /// Current frame of the animation
    /// </summary>
    public double CurrentFrame { get; init; }

    /// <summary>
    /// Total frames for the entire animation
    /// </summary>
    public double TotalFrames { get; init; }

};