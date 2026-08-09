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
    /// No preview formats are enabled.
    /// </summary>
    None = 0,

    /// <summary>
    /// Renders the report as browser HTML.
    /// </summary>
    Html = 1,

    /// <summary>
    /// Renders the report as PDF content.
    /// </summary>
    Pdf = 2
}

internal static class ReportPreviewFormatResolver
{
    private const ReportPreviewFormat SupportedFormats = ReportPreviewFormat.Html | ReportPreviewFormat.Pdf;

    public static ReportPreviewFormat Normalize( ReportPreviewFormat formats )
        => formats & SupportedFormats;

    public static bool IsEnabled( ReportPreviewFormat formats, ReportPreviewFormat format )
        => format is ReportPreviewFormat.Html or ReportPreviewFormat.Pdf
            && ( Normalize( formats ) & format ) == format;

    public static ReportPreviewFormat Resolve( ReportPreviewFormat format, ReportPreviewFormat formats, ReportPreviewFormat fallback = ReportPreviewFormat.Html )
    {
        formats = Normalize( formats );

        if ( IsEnabled( formats, format ) )
            return format;

        if ( IsEnabled( formats, fallback ) )
            return fallback;

        if ( IsEnabled( formats, ReportPreviewFormat.Html ) )
            return ReportPreviewFormat.Html;

        if ( IsEnabled( formats, ReportPreviewFormat.Pdf ) )
            return ReportPreviewFormat.Pdf;

        return ReportPreviewFormat.Html;
    }
}