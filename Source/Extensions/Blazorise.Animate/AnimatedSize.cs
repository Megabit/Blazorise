namespace Blazorise.Animate;

/// <summary>
/// Defines which size dimension should be animated together with the configured animation.
/// </summary>
public enum AnimatedSize
{
    /// <summary>
    /// Does not animate size dimensions.
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