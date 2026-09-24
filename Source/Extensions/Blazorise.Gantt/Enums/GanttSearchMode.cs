namespace Blazorise.Gantt;

/// <summary>
/// Defines which related items are included in Gantt search results.
/// </summary>
public enum GanttSearchMode
{
    /// <summary>
    /// Includes matching items and their ancestors.
    /// </summary>
    Match,

    /// <summary>
    /// Includes matching items, their ancestors, and all descendants of matching items.
    /// </summary>
    /// <remarks>
    /// Ancestors retained for context do not include unrelated descendants.
    /// </remarks>
    Subtree,
}