namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Card"/> component.
/// </summary>
public record ThemeCardOptions : ThemeBasicOptions
{
    /// <summary>
    /// Defines the top radius of the image element placed inside of the card.
    /// </summary>
    public string ImageTopRadius { get; set; } = "calc(.25rem - 1px)";
}