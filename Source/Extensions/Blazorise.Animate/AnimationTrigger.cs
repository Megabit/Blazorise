namespace Blazorise.Animate;

/// <summary>
/// Defines when the <see cref="Animate"/> component should run the configured animation.
/// </summary>
public enum AnimationTrigger
{
    /// <summary>
    /// Runs the animation when the component element enters the viewport.
    /// </summary>
    InView,

    /// <summary>
    /// Runs the animation as soon as the component element is rendered.
    /// </summary>
    Render,
}