namespace Blazorise.Gantt;

/// <summary>
/// Event arguments for gantt column visibility changes.
/// </summary>
public class GanttColumnVisibilityChangedEventArgs
{
    /// <summary>
    /// Creates a new <see cref="GanttColumnVisibilityChangedEventArgs"/>.
    /// </summary>
    public GanttColumnVisibilityChangedEventArgs( string key, bool visible )
    {
        Key = key;
        Visible = visible;
    }

    /// <summary>
    /// Column key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// New visibility value.
    /// </summary>
    public bool Visible { get; }
}