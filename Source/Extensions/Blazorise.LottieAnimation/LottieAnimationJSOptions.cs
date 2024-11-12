namespace Blazorise.LottieAnimation;

/// <summary>
/// Represents JavaScript options for configuring a Lottie animation.
/// </summary>
public class LottieAnimationJSOptions
{
    /// <summary>
    /// Gets or sets the file path or URL to the Lottie animation JSON data.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the loop configuration for the animation. Determines whether and how the animation loops.
    /// </summary>
    public LoopConfiguration Loop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the animation should start playing automatically.
    /// </summary>
    public bool Autoplay { get; set; }

    /// <summary>
    /// Gets or sets the renderer type for the animation (e.g., "svg", "canvas", or "html").
    /// </summary>
    public LottieAnimationRenderer Renderer { get; set; }

    /// <summary>
    /// Gets or sets the playback direction of the animation. A value of <c>1</c> plays forward, and <c>-1</c> plays in reverse.
    /// </summary>
    public LottieAnimationDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets the playback speed of the animation. A value of <c>1</c> is normal speed, while values greater or less than <c>1</c> adjust the speed proportionally.
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to send the current frame index during playback.
    /// </summary>
    public bool SendCurrentFrame { get; set; }
}