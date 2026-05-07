using System.Collections.Generic;

namespace Blazorise.Animate;

/// <summary>
/// Defines an animation from a name and a set of keyframes.
/// </summary>
public class AnimationDefinition : IAnimationDefinition
{
    /// <summary>
    /// Initializes a new instance of the animation definition.
    /// </summary>
    /// <param name="name">Animation name.</param>
    /// <param name="keyframes">Animation keyframes.</param>
    public AnimationDefinition( string name, IReadOnlyList<AnimationFrame> keyframes )
    {
        Name = name;
        Keyframes = keyframes;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IReadOnlyList<AnimationFrame> Keyframes { get; }
}