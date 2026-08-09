namespace Blazorise.Reporting;

/// <summary>
/// Describes how a custom report element appears and behaves in the designer.
/// </summary>
public sealed class ReportElementDescriptor
{
    #region Properties

    /// <summary>
    /// Stable, application-wide element type name.
    /// </summary>
    public string TypeName { get; init; }

    /// <summary>
    /// Display name shown in the toolbox and Report Explorer.
    /// </summary>
    public string DisplayName { get; init; }

    /// <summary>
    /// Toolbox group containing the element.
    /// </summary>
    public string Category { get; init; } = "Custom";

    /// <summary>
    /// Properties pane group title. The display name is used when not specified.
    /// </summary>
    public string PropertiesGroupName { get; init; }

    /// <summary>
    /// Icon shown beside the element.
    /// </summary>
    public IconName Icon { get; init; } = IconName.PuzzlePiece;

    /// <summary>
    /// Default width in points.
    /// </summary>
    public double Width { get; init; } = 120;

    /// <summary>
    /// Default height in points.
    /// </summary>
    public double Height { get; init; } = 36;

    /// <summary>
    /// Current plugin-owned properties schema version.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Indicates whether the element is shown in the toolbox.
    /// </summary>
    public bool ShowInToolbox { get; init; } = true;

    /// <summary>
    /// Designer features supported by the element.
    /// </summary>
    public ReportElementCapabilities Capabilities { get; init; } = ReportElementCapabilities.Default;

    #endregion
}