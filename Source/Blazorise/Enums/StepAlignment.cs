namespace Blazorise;

/// <summary>
/// Defines how horizontal steps are distributed across the available width.
/// </summary>
public enum StepAlignment
{
    /// <summary>
    /// Preserves the provider's default layout.
    /// </summary>
    Default,

    /// <summary>
    /// Groups items at the start of the list.
    /// </summary>
    Start,

    /// <summary>
    /// Groups items in the center of the list.
    /// </summary>
    Center,

    /// <summary>
    /// Groups items at the end of the list.
    /// </summary>
    End,

    /// <summary>
    /// Distributes free space between steps, retaining the provider's marker and caption sizes.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Distributes equal space around each step, retaining the provider's marker and caption sizes.
    /// </summary>
    Justified,
}