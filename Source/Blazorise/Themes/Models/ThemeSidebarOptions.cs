namespace Blazorise;

/// <summary>
/// Defines the theme options for the Sidebar component.
/// </summary>
public record ThemeSidebarOptions
{
    /// <summary>
    /// Gets or sets the width of the sidebar.
    /// </summary>
    public string Width { get; set; } = "230px";

    /// <summary>
    /// Gets or sets the background color of the sidebar.
    /// </summary>
    public string BackgroundColor { get; set; } = "#343a40";

    /// <summary>
    /// Gets or sets the text color of the sidebar items.
    /// </summary>
    public string Color { get; set; } = "#ced4da";
}