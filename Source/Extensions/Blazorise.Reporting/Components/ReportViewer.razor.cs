#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Configures preview behavior for the containing report.
/// </summary>
public partial class ReportViewer : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var optionsChanged = ReportContext is not null
            && ( parameters.IsParameterChanged( PreviewFormat )
                || parameters.IsParameterChanged( DefaultPreviewFormat )
                || parameters.IsParameterChanged( AllowPrint )
                || parameters.IsParameterChanged( AllowDownload )
                || parameters.IsParameterChanged( PdfPreviewTemplate ) );

        await base.SetParametersAsync( parameters );

        if ( optionsChanged )
        {
            UpdateOptions();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        UpdateOptions();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ReportContext?.UnregisterViewer( this );
        ReportContext = null;
    }

    private void UpdateOptions()
    {
        ReportContext?.RegisterViewer( this, new()
        {
            PreviewFormats = PreviewFormat,
            DefaultFormat = DefaultPreviewFormat,
            AllowPrint = AllowPrint,
            AllowDownload = AllowDownload,
            PdfPreviewTemplate = PdfPreviewTemplate,
        } );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Preview formats offered by the report viewer. None disables preview.
    /// </summary>
    [Parameter] public ReportPreviewFormat PreviewFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Preview format selected when preview mode is opened. Must be enabled in <see cref="PreviewFormat"/>.
    /// </summary>
    [Parameter] public ReportPreviewFormat DefaultPreviewFormat { get; set; } = ReportPreviewFormat.Html;

    /// <summary>
    /// Enables print commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowPrint { get; set; } = true;

    /// <summary>
    /// Enables download commands in the viewer toolbar.
    /// </summary>
    [Parameter] public bool AllowDownload { get; set; } = true;

    /// <summary>
    /// Template used to render generated PDF previews.
    /// </summary>
    [Parameter] public RenderFragment<ReportPdfPreviewContext> PdfPreviewTemplate { get; set; }

    #endregion
}