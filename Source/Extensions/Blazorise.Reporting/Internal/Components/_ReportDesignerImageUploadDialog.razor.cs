#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to select and upload an image for a report image element.
/// </summary>
public partial class _ReportDesignerImageUploadDialog
{
    #region Members

    private const long DefaultImageMaxSize = 1024 * 1024 * 5;

    private const int DefaultMaxUploadImageChunkSize = 20 * 1024;

    private FileInput fileInputRef;

    private IFileEntry selectedFile;

    private string previewSource;

    private string errorMessage;

    private bool busy;

    #endregion

    #region Methods

    internal async Task Show()
    {
        await ShowReportModal<_ReportDesignerImageUploadDialog>( parameters =>
        {
            parameters.Add( nameof( ImageAccept ), ImageAccept );
            parameters.Add( nameof( ImageMaxSize ), ImageMaxSize );
            parameters.Add( nameof( MaxUploadImageChunkSize ), MaxUploadImageChunkSize );
            parameters.Add( nameof( SegmentFetchTimeout ), SegmentFetchTimeout );
            parameters.Add( nameof( DisableImageUploadProgressReport ), DisableImageUploadProgressReport );
            parameters.Add( nameof( ImageUploadChanged ), ImageUploadChanged );
            parameters.Add( nameof( ImageUploadStarted ), ImageUploadStarted );
            parameters.Add( nameof( ImageUploadEnded ), ImageUploadEnded );
            parameters.Add( nameof( ImageUploadWritten ), ImageUploadWritten );
            parameters.Add( nameof( ImageUploadProgressed ), ImageUploadProgressed );
            parameters.Add( nameof( ImageUpload ), ImageUpload );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task OnChanged( FileChangedEventArgs eventArgs )
    {
        selectedFile = eventArgs?.Files?.FirstOrDefault();
        previewSource = null;
        errorMessage = null;

        await ImageUploadChanged.InvokeAsync( eventArgs );

        if ( selectedFile is not null )
        {
            try
            {
                previewSource = await CreateDataUri( selectedFile );
            }
            catch ( Exception e )
            {
                errorMessage = e.Message;
            }
        }
    }

    private Task OnStarted( FileStartedEventArgs eventArgs )
    {
        return ImageUploadStarted.InvokeAsync( eventArgs );
    }

    private Task OnEnded( FileEndedEventArgs eventArgs )
    {
        if ( eventArgs is not null && !eventArgs.Success )
            errorMessage = eventArgs.File?.ErrorMessage ?? "Image upload failed.";

        return ImageUploadEnded.InvokeAsync( eventArgs );
    }

    private Task OnWritten( FileWrittenEventArgs eventArgs )
    {
        return ImageUploadWritten.InvokeAsync( eventArgs );
    }

    private Task OnProgressed( FileProgressedEventArgs eventArgs )
    {
        return ImageUploadProgressed.InvokeAsync( eventArgs );
    }

    private async Task Confirm()
    {
        if ( selectedFile is null || busy )
            return;

        busy = true;
        errorMessage = null;

        try
        {
            string source;

            if ( ImageUpload.HasDelegate )
            {
                await ImageUpload.InvokeAsync( new( selectedFile ) );

                source = selectedFile.UploadUrl;

                if ( string.IsNullOrWhiteSpace( source ) )
                {
                    errorMessage = "Image upload did not return an image URL.";
                    return;
                }
            }
            else
            {
                try
                {
                    source = previewSource ?? await CreateDataUri( selectedFile );
                }
                catch ( Exception e )
                {
                    errorMessage = e.Message;
                    return;
                }
            }

            await Confirmed.InvokeAsync( source );
            await CloseReportModal();
        }
        finally
        {
            busy = false;
        }
    }

    private async Task<string> CreateDataUri( IFileEntry file )
    {
        if ( file is null )
            return null;

        using MemoryStream stream = new();
        await file.OpenReadStream( EffectiveImageMaxSize ).CopyToAsync( stream );

        string contentType = string.IsNullOrWhiteSpace( file.Type ) ? "application/octet-stream" : file.Type;

        return $"data:{contentType};base64,{Convert.ToBase64String( stream.ToArray() )}";
    }

    private static string FormatFileSize( long value )
    {
        if ( value < 1024 )
            return $"{value} B";

        double kilobytes = value / 1024d;

        if ( kilobytes < 1024 )
            return $"{kilobytes:0.#} KB";

        return $"{kilobytes / 1024d:0.#} MB";
    }

    #endregion

    #region Properties

    private long EffectiveImageMaxSize => ImageMaxSize > 0 ? ImageMaxSize : DefaultImageMaxSize;

    private int EffectiveMaxUploadImageChunkSize => MaxUploadImageChunkSize > 0 ? MaxUploadImageChunkSize : DefaultMaxUploadImageChunkSize;

    private bool CanConfirm => selectedFile is not null && !busy && string.IsNullOrWhiteSpace( errorMessage );

    private string ConfirmText => ImageUpload.HasDelegate ? "Upload" : "Use image";

    /// <summary>
    /// A comma-separated list of image MIME types accepted by the picker.
    /// </summary>
    [Parameter] public string ImageAccept { get; set; } = "image/png, image/jpeg, image/webp, image/svg+xml";

    /// <summary>
    /// Maximum image size in bytes.
    /// </summary>
    [Parameter] public long ImageMaxSize { get; set; } = DefaultImageMaxSize;

    /// <summary>
    /// Specifies the max chunk size when uploading the image.
    /// </summary>
    [Parameter] public int MaxUploadImageChunkSize { get; set; } = DefaultMaxUploadImageChunkSize;

    /// <summary>
    /// Specifies the segment fetch timeout when uploading the image.
    /// </summary>
    [Parameter] public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Disables progress callbacks while image data is read.
    /// </summary>
    [Parameter] public bool DisableImageUploadProgressReport { get; set; }

    /// <summary>
    /// Raised when the selected image changes.
    /// </summary>
    [Parameter] public EventCallback<FileChangedEventArgs> ImageUploadChanged { get; set; }

    /// <summary>
    /// Raised when reading an image starts.
    /// </summary>
    [Parameter] public EventCallback<FileStartedEventArgs> ImageUploadStarted { get; set; }

    /// <summary>
    /// Raised when reading an image ends.
    /// </summary>
    [Parameter] public EventCallback<FileEndedEventArgs> ImageUploadEnded { get; set; }

    /// <summary>
    /// Raised when an image chunk is read.
    /// </summary>
    [Parameter] public EventCallback<FileWrittenEventArgs> ImageUploadWritten { get; set; }

    /// <summary>
    /// Raised when image read progress changes.
    /// </summary>
    [Parameter] public EventCallback<FileProgressedEventArgs> ImageUploadProgressed { get; set; }

    /// <summary>
    /// Raised when the dialog upload action is confirmed.
    /// </summary>
    [Parameter] public EventCallback<FileUploadEventArgs> ImageUpload { get; set; }

    /// <summary>
    /// Raised when the image source is resolved.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        selectedFile = null;
        previewSource = null;
        errorMessage = null;
        busy = false;
    }

    #endregion
}