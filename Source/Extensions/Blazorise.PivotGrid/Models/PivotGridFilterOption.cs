namespace Blazorise.PivotGrid;

/// <summary>
/// Represents one selectable PivotGrid filter value.
/// </summary>
public class PivotGridFilterOption
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridFilterOption"/>.
    /// </summary>
    public PivotGridFilterOption( string key, string text )
    {
        Key = key;
        Text = text;
    }

    /// <summary>
    /// Gets the stable value key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the displayed value text.
    /// </summary>
    public string Text { get; }
}