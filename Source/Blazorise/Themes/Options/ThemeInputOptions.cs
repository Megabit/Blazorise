namespace Blazorise;

/// <summary>
/// Defines the theme options for the input component(s).
/// </summary>
public record ThemeInputOptions : ThemeBasicOptions
{
    /// <summary>
    /// Gets or sets the input text color.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Gets or sets the checkbox color.
    /// </summary>
    public string CheckColor { get; set; }

    /// <summary>
    /// Gets or sets the slider text color.
    /// </summary>
    public string SliderColor { get; set; }

    /// <summary>
    /// Gets or sets the global size for input components.
    /// </summary>
    public Size? Size { get; set; }

    /// <inheritdoc/>
    public override bool HasOptions()
    {
        return !string.IsNullOrEmpty( Color )
               || !string.IsNullOrEmpty( CheckColor )
               || !string.IsNullOrEmpty( SliderColor )
               || Size is not null
               || base.HasOptions();
    }
}