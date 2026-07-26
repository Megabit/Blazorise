#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Defines the preview formats supported by a report viewer.
/// </summary>
[Flags]
public enum ReportPreviewFormat
{
    /// <summary>
    /// Renders the report as browser HTML.
    /// </summary>
    Html = 1,

    /// <summary>
    /// Renders the report as PDF content.
    /// </summary>
    Pdf = 2
}