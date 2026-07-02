using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes the printable area inset from each page edge.
/// </summary>
public sealed class ReportPageMarginsDefinition
{
    /// <summary>
    /// Left page margin in points.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Top page margin in points.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Right page margin in points.
    /// </summary>
    public double Right { get; set; }

    /// <summary>
    /// Bottom page margin in points.
    /// </summary>
    public double Bottom { get; set; }
}