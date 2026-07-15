#region Using directives
using Blazorise.Reporting.Internal;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes persistent report designer settings.
/// </summary>
public sealed class ReportDesignerDefinition
{
    #region Properties

    /// <summary>
    /// Enables grid-aligned movement and resizing in the designer.
    /// </summary>
    public bool SnapToGrid { get; set; } = true;

    /// <summary>
    /// Grid size used by designer movement and resizing, in points.
    /// </summary>
    public double GridSize { get; set; } = ReportLayoutGeometry.DefaultGridSize;

    /// <summary>
    /// Shows measurement rulers around the report designer page.
    /// </summary>
    public bool ShowRulers { get; set; } = true;

    /// <summary>
    /// Shows fine-grained measurement ruler ticks.
    /// </summary>
    public bool ShowFineRulerTicks { get; set; }

    /// <summary>
    /// Shows design-time warnings when sibling report elements overlap.
    /// </summary>
    public bool ShowCollisionWarnings { get; set; } = true;

    /// <summary>
    /// Band presentation used by the designer.
    /// </summary>
    public ReportBandMode BandMode { get; set; } = ReportBandMode.Classic;

    #endregion
}