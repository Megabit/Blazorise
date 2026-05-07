using System.Collections.Generic;

namespace Blazorise.Animate;

/// <summary>
/// Defines an animation that provides its own keyframes.
/// </summary>
public interface IAnimationDefinition : IAnimation
{
    /// <summary>
    /// Gets the keyframes used to run the animation.
    /// </summary>
    IReadOnlyList<AnimationFrame> Keyframes { get; }
}