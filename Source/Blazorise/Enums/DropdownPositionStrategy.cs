namespace Blazorise.Enums;

/// <summary>
/// Defines the position strategy for the floating component.
/// <para>See : https://floating-ui.com/docs/computeposition#strategy</para>
/// </summary>
public enum DropdownPositionStrategy
{
    /// <summary>
    /// The floating element is positioned relative to its nearest positioned ancestor. With most layouts, this usually requires the browser to do the least work when updating the position.
    /// </summary>
    Absolute,
    /// <summary>
    /// The floating element is positioned relative to its nearest containing block (usually the viewport). This is useful when the reference element is also fixed to reduce jumpiness with positioning while scrolling.
    /// </summary>
    Fixed
}
