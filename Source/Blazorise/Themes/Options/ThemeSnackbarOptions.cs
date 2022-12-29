namespace Blazorise;

/// <summary>
/// Defines the theme options for the Snackbar component.
/// </summary>
public record ThemeSnackbarOptions
{
    /// <summary>
    /// Default snackbar color.
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Default text color.
    /// </summary>
    public string TextColor { get; set; }

    /// <summary>
    /// Default button color.
    /// </summary>
    public string ButtonColor { get; set; }

    /// <summary>
    /// Default button hover color.
    /// </summary>
    public string ButtonHoverColor { get; set; }

    /// <summary>
    /// Defines a level of background color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? VariantBackgroundColorLevel { get; set; } = -3;

    //public int VariantTextColorLevel { get; set; } = 6;

    //public int VariantButtonColorLevel { get; set; } = 8;

    //public int VariantButtonHoverColorLevel { get; set; } = 4;
}