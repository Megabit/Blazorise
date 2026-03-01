namespace Blazorise.Gantt;

/// <summary>
/// Model used by gantt column picker.
/// </summary>
public class GanttColumnPickerItem
{
    /// <summary>
    /// Gets or sets column key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets column title.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets current visibility.
    /// </summary>
    public bool Visible { get; set; }
}