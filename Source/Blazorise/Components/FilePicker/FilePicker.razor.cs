#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using static System.Net.WebRequestMethods;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper component build upon <see cref="FileEdit"/> component providing extra file uploading features.
/// </summary>
public partial class FilePicker : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Tracks the current file being uploaded.
    /// </summary>
    IFileEntry fileBeingUploaded;

    /// <summary>
    /// Holds the reference to the confirmation dialog component.
    /// </summary>
    private _FilePickerConfirmModal filePickerConfirmModalRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ElementContainerId = IdGenerator.Generate;
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }


    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await JSFilePickerModule.Initialize( ElementRef, ElementContainerId );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSFilePickerModule.SafeDestroy( ElementRef, ElementId );

            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Handles the localization changed event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the localization event.</param>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Gets progress for percentage display.
    /// </summary>
    /// <returns></returns>
    public int GetProgressPercentage()
        => (int)( FileEdit.GetCurrentProgress().Progress * 100d );

    /// <summary>
    /// Tracks whether the current file is being uploaded.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool IsFileBeingUploaded( IFileEntry file )
        => file.IsEqual( fileBeingUploaded );

    /// <summary>
    /// Tracks whether the FilePicker is busy and user interaction should be disabled.
    /// </summary>
    /// <returns></returns>
    public bool IsBusy()
        => fileBeingUploaded is not null;

    /// <summary>
    /// Tracks whether the FilePicker has files ready to upload.
    /// </summary>
    /// <returns></returns>
    public bool IsUploadReady()
        => FileEdit.Files?.Any( x => x.Status == FileEntryStatus.Ready ) ?? false;

    /// <summary>
    /// Converts the file size in bytes into a proper human readable format.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetFileSizeReadable( IFileEntry file )
        => Formaters.GetBytesReadable( file.Size );


    /// <summary>
    /// Gets the File Status
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public string GetFileStatus( IFileEntry file )
    {
        switch ( file.Status )
        {
            case FileEntryStatus.Ready:
                return GetLocalizedString( "Ready to upload" );
            case FileEntryStatus.Uploaded:
                return GetLocalizedString( "Uploaded successfully" );
            case FileEntryStatus.ExceedsMaximumSize:
                return GetLocalizedString( "File size is too large" );
            case FileEntryStatus.Error:
                return GetLocalizedString( "Error uploading" );
            default:
                break;
        }

        return string.Empty;
    }

    /// <summary>
    /// Gets a FilePicker Localized string.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetLocalizedString( string name )
    {
        if ( FilePickerLocalizer is not null )
            return FilePickerLocalizer.Invoke( name );

        return Localizer.GetString( name );
    }
    /// <summary>
    /// Removes the file from FileEdit.
    /// </summary>
    /// <param name="file">The file entry to remove.</param>
    /// <param name="confirm">Wether to show a confirmation popup.</param>
    public Task RemoveFile( IFileEntry file, bool confirm = false )
    {
        var removeFileAction = () => FileEdit.RemoveFile( file.Id ).AsTask();

        if ( confirm )
            return filePickerConfirmModalRef.OpenModal( _FilePickerConfirmModal.ConfirmOperation.RemoveFile, removeFileAction );
        else
            return removeFileAction.Invoke();
    }

    /// <summary>
    /// Clears the FileEdit by resetting the state.
    /// </summary>
    /// <param name="confirm">Wether to show a confirmation popup.</param>
    public Task Clear( bool confirm = false )
    {
        var clearAction = async () =>
        {
            await Cancel();
            await FileEdit.Reset();
        };

        if ( confirm )
            return filePickerConfirmModalRef.OpenModal( _FilePickerConfirmModal.ConfirmOperation.Clear, clearAction );
        else
            return clearAction.Invoke();
    }

    /// <summary>
    /// Uploads the current files.
    /// </summary>
    /// <returns></returns>
    public async Task UploadAll()
    {
        if ( Upload.HasDelegate && !FileEdit.Files.IsNullOrEmpty() )
        {
            cts = new();
            foreach ( var file in FileEdit.Files )
            {
                await UploadFile( file );
            }
        }
    }

    /// <summary>
    /// Uploads a single file if the status is set to ready.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="forceUpload">Forces file upload attempt.</param>
    /// <returns></returns>
    public async Task UploadFile( IFileEntry file, bool forceUpload = false )
    {
        cts ??= new();
        if ( forceUpload || ( file.Status == FileEntryStatus.Ready && !cts.IsCancellationRequested ) )
            await Upload.InvokeAsync( new( file ) );
    }

    /// <summary>
    /// Cancels any ongoing upload operation.
    /// </summary>
    /// <param name="confirm">Wether to show a confirmation popup.</param>
    public Task Cancel( bool confirm = false )
    {
        var cancelAction = new Func<Task>( async () =>
        {
            cts?.Cancel();

            if ( fileBeingUploaded is not null )
                await fileBeingUploaded.Cancel();
        } );

        if ( confirm )
            return filePickerConfirmModalRef.OpenModal( _FilePickerConfirmModal.ConfirmOperation.CancelUpload, cancelAction );
        else
            return cancelAction.Invoke();
    }

    /// <summary>
    /// Internal method. Facilitates named binding.
    /// </summary>
    /// <returns></returns>
    private Task RemoveFileWithConfirm( IFileEntry file )
        => RemoveFile( file, true );

    /// <summary>
    /// Internal method. Facilitates named binding.
    /// </summary>
    /// <returns></returns>
    private Task ClearWithConfirm()
        => Clear( true );

    /// <summary>
    /// Internal method. Facilitates named binding.
    /// </summary>
    /// <returns></returns>
    private Task CancelWithConfirm()
        => Cancel( true );

    private CancellationTokenSource cts;

    /// <summary>
    /// FilePicker's handling of the Started Event.
    /// </summary>
    /// <param name="fileStartedEventArgs"></param>
    /// <returns></returns>
    protected Task OnStarted( FileStartedEventArgs fileStartedEventArgs )
    {
        fileBeingUploaded = fileStartedEventArgs.File;
        return Started.InvokeAsync( fileStartedEventArgs );
    }

    /// <summary>
    /// FilePicker's handling of the Changed Event.
    /// </summary>
    /// <param name="fileChangedEventArgs"></param>
    /// <returns></returns>
    protected Task OnChanged( FileChangedEventArgs fileChangedEventArgs )
        => Changed.InvokeAsync( fileChangedEventArgs );

    /// <summary>
    /// FilePicker's handling of the Ended Event.
    /// </summary>
    /// <param name="fileEndedEventArgs"></param>
    /// <returns></returns>
    protected Task OnEnded( FileEndedEventArgs fileEndedEventArgs )
    {
        fileBeingUploaded = null;
        return Ended.InvokeAsync( fileEndedEventArgs );
    }

    /// <summary>
    /// FilePicker's handling of the Progressed Event.
    /// </summary>
    /// <param name="fileProgressedEventArgs"></param>
    /// <returns></returns>
    protected Task OnProgressed( FileProgressedEventArgs fileProgressedEventArgs )
    {
        return Progressed.InvokeAsync( fileProgressedEventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Accesses the FileEdit
    /// </summary>
    public FileEdit FileEdit;

    /// <summary>
    /// Gets or sets the main container element id.
    /// </summary>
    private string ElementContainerId { get; set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the localized browse button text.
    /// </summary>
    protected string DragAndDropString
    {
        get
        {
            var localizationString = Directory ? "Or drop files or folders here" : "Or drop files here";

            return GetLocalizedString( localizationString );
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="IJSFilePickerModule"/> instance.
    /// </summary>
    [Inject] public IJSFilePickerModule JSFilePickerModule { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer{FilePicker}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<FilePicker> Localizer { get; set; }

    /// <summary>
    /// Input content.
    /// </summary>
    [Parameter] public RenderFragment ChildTemplate { get; set; }

    /// <summary>
    /// Placeholder for validation messages.
    /// </summary>
    [Parameter] public RenderFragment Feedback { get; set; }

    /// <summary>
    /// Enables the multiple file selection.
    /// </summary>
    [Parameter] public bool Multiple { get; set; }

    /// <summary>
    /// Sets the placeholder for the empty file input.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Specifies the types of files that the input accepts.
    /// <see href="https://www.w3schools.com/tags/att_input_accept.asp"/>
    /// </summary>
    [Parameter] public string Filter { get; set; }

    /// <summary>
    /// Gets or sets the max chunk size when uploading the file.
    /// </summary>
    [Parameter] public int MaxChunkSize { get; set; } = 20 * 1024;

    /// <summary>
    /// Maximum file size in bytes, checked before starting upload (note: never trust client, always check file
    /// size at server-side). Defaults to <see cref="long.MaxValue"/>.
    /// </summary>
    [Parameter] public long MaxFileSize { get; set; } = long.MaxValue;

    /// <summary>
    /// Gets or sets the Segment Fetch Timeout when uploading the file.
    /// </summary>
    [Parameter] public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Occurs every time the selected file has changed, including when the reset operation is executed.
    /// </summary>
    [Parameter] public EventCallback<FileChangedEventArgs> Changed { get; set; }

    /// <summary>
    /// Occurs when an individual file upload has started.
    /// </summary>
    [Parameter] public EventCallback<FileStartedEventArgs> Started { get; set; }

    /// <summary>
    /// Occurs when an individual file upload has ended.
    /// </summary>
    [Parameter] public EventCallback<FileEndedEventArgs> Ended { get; set; }

    /// <summary>
    /// Occurs every time the part of file has being written to the destination stream.
    /// </summary>
    [Parameter] public EventCallback<FileWrittenEventArgs> Written { get; set; }

    /// <summary>
    /// Notifies the progress of file being written to the destination stream.
    /// </summary>
    [Parameter] public EventCallback<FileProgressedEventArgs> Progressed { get; set; }

    /// <summary>
    /// Occurs once the FilePicker's Upload is triggered for every file.
    /// </summary>
    [Parameter] public EventCallback<FileUploadEventArgs> Upload { get; set; }

    /// <summary>
    /// If true file input will be automatically reset after it has being uploaded.
    /// </summary>
    [Parameter] public bool AutoReset { get; set; } = false;

    /// <summary>
    /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    [Parameter] public TextLocalizerHandler FilePickerLocalizer { get; set; }

    /// <summary>
    /// Provides a custom file content.
    /// </summary>
    [Parameter] public RenderFragment<FilePickerFileContext> FileTemplate { get; set; }

    /// <summary>
    /// Provides a custom content for upload, clear and cancel buttons.
    /// </summary>
    [Parameter] public RenderFragment<FilePickerButtonsContext> ButtonsTemplate { get; set; }

    /// <summary>
    /// Gets or Sets FilePicker's show mode.
    /// Defaults to <see cref="FilePickerShowMode.List"/>
    /// </summary>
    [Parameter] public FilePickerShowMode ShowMode { get; set; } = FilePickerShowMode.List;

    /// <summary>
    /// Gets or sets whether report progress should be disabled. By enabling this setting, Progressed and Written callbacks won't be called. Internal file progress won't be tracked.
    /// <para>This setting can speed up file transfer considerably.</para>
    /// </summary>
    [Parameter] public bool DisableProgressReport { get; set; } = false;

    /// <summary>
    /// Gets or Sets whether file picker should upload directories.
    /// </summary>
    [Parameter] public bool Directory { get; set; }

    /// <summary>
    /// Add the readonly boolean attribute on an input to prevent modification of the input value.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    #endregion
}