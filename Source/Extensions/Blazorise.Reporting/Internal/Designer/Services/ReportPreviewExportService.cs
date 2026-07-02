#region Using directives
using System;
using System.IO;
using Blazorise.Pdf;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportPreviewExportService
{
    #region Methods

    internal bool SupportsPreviewFormat( ReportPreviewFormat? previewFormats, ReportViewerOptions viewerOptions, ReportPreviewFormat format )
    {
        return ( previewFormats ?? viewerOptions.PreviewFormats ).HasFlag( format );
    }

    internal PdfDocumentDefinition BuildPdfDocument( ReportDefinition definition, object data )
    {
        return ReportPdfDocumentBuilder.Build( definition, data );
    }

    internal string ResolvePdfFileName( ReportDefinition definition )
    {
        string name = string.IsNullOrWhiteSpace( definition?.Name ) ? "report" : definition.Name.Trim();

        foreach ( char invalidCharacter in Path.GetInvalidFileNameChars() )
        {
            name = name.Replace( invalidCharacter, '-' );
        }

        return name.EndsWith( ".pdf", StringComparison.OrdinalIgnoreCase ) ? name : $"{name}.pdf";
    }

    #endregion
}