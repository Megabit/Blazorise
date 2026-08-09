namespace Blazorise;

/// <summary>
/// Defines how properties are arranged in a <see cref="PropertyGridView"/>.
/// </summary>
public enum PropertyGridViewMode
{
    /// <summary>
    /// Properties are displayed in their schema groups and original order.
    /// </summary>
    Categorized,

    /// <summary>
    /// Visible properties are displayed in a single group ordered by label.
    /// </summary>
    Alphabetical,
}