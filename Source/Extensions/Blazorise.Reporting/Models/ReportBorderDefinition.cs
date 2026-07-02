using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes border settings for report elements.
/// </summary>
public sealed class ReportBorderDefinition
{
    /// <summary>
    /// Border color applied by the designer appearance editor.
    /// </summary>
    public ReportColor Color { get; set; }

    /// <summary>
    /// Border width applied around the element.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Border radius applied to the element corners.
    /// </summary>
    public double? Radius { get; set; }
}