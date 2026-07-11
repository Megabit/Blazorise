#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes shared state for a single visual element placed on a report band.
/// </summary>
public abstract class ReportElementDefinition
{
    /// <summary>
    /// Stable identifier used by designer selection and persisted state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly element name shown in the designer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Element kind rendered by the designer and preview.
    /// </summary>
    public abstract ReportElementType Type { get; }

    /// <summary>
    /// Horizontal position within the containing band, in points.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Vertical position within the containing band, in points.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Element width in points.
    /// </summary>
    public double Width { get; set; } = 90;

    /// <summary>
    /// Element height in points.
    /// </summary>
    public double Height { get; set; } = 18;

    /// <summary>
    /// Allows text content to expand the element vertically when rendered.
    /// </summary>
    public ReportValue<bool> CanGrow { get; set; } = false;

    /// <summary>
    /// Prevents the element from being edited on the designer surface and rendered in preview output.
    /// </summary>
    public ReportValue<bool> Suppress { get; set; } = false;

    /// <summary>
    /// Overrides the report-level snap-to-grid behavior for this element. A null value inherits the report setting.
    /// </summary>
    public ReportValue<bool?> SnapToGrid { get; set; } = (bool?)null;

    /// <summary>
    /// Font settings applied to text rendered by the element.
    /// </summary>
    public ReportFontDefinition Font { get; set; } = new();

    /// <summary>
    /// Fill and opacity settings applied by the designer appearance editor.
    /// </summary>
    public ReportAppearanceDefinition Appearance { get; set; } = new();

    /// <summary>
    /// Border settings applied around the element.
    /// </summary>
    public ReportBorderDefinition Border { get; set; } = new();

    /// <summary>
    /// CSS classes applied when the element is rendered.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Inline style applied when the element is rendered.
    /// </summary>
    public string Style { get; set; }

}