namespace Blazorise.Animate;

/// <summary>
/// Identifies an animation preset consumed by <see cref="Animate"/>.
/// </summary>
public interface IAnimation
{
    /// <summary>
    /// CSS animation identifier associated with the preset.
    /// </summary>
    string Name { get; }
}