namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Steps"/> component.
/// </summary>
public record ThemeStepsOptions
{
    /// <summary>
    /// Gets or sets the <see cref="Step"/> icon color.
    /// </summary>
    public string StepsItemIconColor { get; set; } = "#adb5bd";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> icon color for completed state.
    /// </summary>
    public string StepsItemIconCompleted { get; set; } = "#007bff";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> icon contrast color for completed state.
    /// </summary>
    public string StepsItemIconCompletedYiq { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> icon color for active state.
    /// </summary>
    public string StepsItemIconActive { get; set; } = "#007bff";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> icon contrast color for active state.
    /// </summary>
    public string StepsItemIconActiveYiq { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> text color.
    /// </summary>
    public string StepsItemTextColor { get; set; } = "#adb5bd";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> text color for completed state.
    /// </summary>
    public string StepsItemTextCompleted { get; set; } = "#28a745";

    /// <summary>
    /// Gets or sets the <see cref="Step"/> text color for active state.
    /// </summary>
    public string StepsItemTextActive { get; set; } = "#28a745";
}