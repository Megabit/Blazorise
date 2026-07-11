#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes fill and opacity settings for report elements.
/// </summary>
public sealed class ReportAppearanceDefinition
{
    /// <summary>
    /// Background color applied to the element fill.
    /// </summary>
    public ReportColor BackgroundColor { get; set; }

    /// <summary>
    /// Element opacity from 0 to 1.
    /// </summary>
    public double? Opacity { get; set; }
}