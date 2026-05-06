namespace Blazorise.Animate;

/// <summary>
/// Defines which layout dimension should be animated together with the configured animation.
/// </summary>
public enum AnimationLayout
{
    /// <summary>
    /// Does not animate layout dimensions.
    /// </summary>
    None,

    /// <summary>
    /// Animates the element width.
    /// </summary>
    Width,

    /// <summary>
    /// Animates the element height.
    /// </summary>
    Height,
}