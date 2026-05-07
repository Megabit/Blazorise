namespace Blazorise.Animate;

/// <summary>
/// Defines an easing that provides its own runtime value.
/// </summary>
public interface IEasingDefinition : IEasing
{
    /// <summary>
    /// Gets the easing value used by the animation runtime.
    /// </summary>
    object Value { get; }
}