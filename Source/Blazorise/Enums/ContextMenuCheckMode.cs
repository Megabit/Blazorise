namespace Blazorise;

/// <summary>
/// Defines how a <see cref="ContextMenuGroup"/> coordinates checked item state.
/// </summary>
public enum ContextMenuCheckMode
{
    /// <summary>
    /// The group does not coordinate checked state.
    /// </summary>
    None,

    /// <summary>
    /// The group allows only one checked item based on the selected value.
    /// </summary>
    Radio,
}