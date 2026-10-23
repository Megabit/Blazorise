namespace Blazorise.DataGrid;

/// <summary>
/// Defines how loading content is displayed in a data grid.
/// </summary>
public enum DataGridLoadingDisplayMode
{
    /// <summary>
    /// Replaces the data rows with a supplied loading template, or displays no loading content when no template is supplied.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Does not display loading content or a backdrop.
    /// </summary>
    None = 1,

    /// <summary>
    /// Displays loading content over the existing grid and disables interaction with the covered content.
    /// </summary>
    Overlay = 2,

    /// <summary>
    /// Replaces the data rows with loading content and hides the pager while loading.
    /// </summary>
    Replace = 3,
}