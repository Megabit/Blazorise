namespace Blazorise.Reporting;

/// <summary>
/// Defines application-wide defaults for Blazorise Reporting components.
/// </summary>
public sealed class ReportOptions
{
    /// <summary>
    /// Preview formats available to report viewers by default. None disables preview.
    /// </summary>
    public ReportPreviewFormat PreviewFormats { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Preview format selected when a report first enters preview mode. Must be enabled in <see cref="PreviewFormats"/>.
    /// </summary>
    public ReportPreviewFormat DefaultPreviewFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Strategy used when declarative report markup and persisted definitions are both available.
    /// </summary>
    public ReportDefinitionMode DefinitionMode { get; set; } = ReportDefinitionMode.SeedWhenEmpty;

    /// <summary>
    /// Gets or sets whether reports can be edited using the interactive designer.
    /// </summary>
    public bool Editable { get; set; }

    /// <summary>
    /// Enables print commands in report viewers by default.
    /// </summary>
    public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Enables download commands in report viewers by default.
    /// </summary>
    public bool AllowDownload { get; set; } = true;
}