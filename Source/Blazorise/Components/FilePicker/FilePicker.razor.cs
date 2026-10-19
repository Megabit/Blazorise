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
/// A wrapper component build upon <see cref="FileInput"/> component providing extra file uploading features.
/// </summary>
public partial class FilePicker : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Tracks the current file being uploaded.
    /// </summary>
    IFileEntry fileBeingUploaded;

    /// <summary>
    /// Tracks the browser selection already reconciled with Files. FileInput can render during
    /// validation before delivering Changed, so its newer selection must not be pruned yet.
    /// </summary>
    private IFileEntry[] reconciledBrowserFiles = Array.Empty<IFileEntry>();

    /// <summary>
    /// Holds the reference to the confirmation dialog component.
    /// </summary>
    protected _FilePickerConfirmModal filePickerConfirmModalRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Files, out var paramFiles );
        parameters.TryGetParameter( MaxFileSize, out var paramMaxFileSize );

        // Supplied collections can contain entries whose content changed in place.
        if ( paramFiles.Defined || paramMaxFileSize.Changed )
        {
            var effectiveFiles = paramFiles.GetValueOrDefault( Files ) ?? Array.Empty<IFileEntry>();
            var effectiveMaxFileSize = paramMaxFileSize.GetValueOrDefault( MaxFileSize );

            foreach ( var file in effectiveFiles.OfType<FileEntry>() )
            {
                if ( file.HasSuppliedContent && file.Status is FileEntryStatus.Ready or FileEntryStatus.ExceedsMaximumSize )
                {
                    file.Status = file.Size > effectiveMaxFileSize ? FileEntryStatus.ExceedsMaximumSize : FileEntryStatus.Ready;
                }
            }
        }

        if ( Rendered && paramFiles.Defined
            && ShouldReconcileFiles( paramFiles.Value ?? Array.Empty<IFileEntry>() ) )
        {
            ExecuteAfterRender( ReconcileFiles );
        }

        return base.SetParametersAsync( parameters );
    }

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
        if ( disposing )
        {
            await Cancel();
        }

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
        => (int)( FileInputRef.GetCurrentProgress().Progress * 100d );

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
        => EffectiveFiles.Any( file => ShouldUploadFile( file ) && CanUploadFile( file ) );

    /// <summary>
    /// Determines whether the file has content available for upload.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    protected bool CanUploadFile( IFileEntry file )
        => file is not null && file.Size <= MaxFileSize && ( file is not FileEntry entry || entry.HasContent );

    private bool IsBrowserFile( IFileEntry file )
        => file is FileEntry entry && FileInputRef is not null && ReferenceEquals( entry.Owner, FileInputRef );

    private static bool ShouldUploadFile( IFileEntry file )
        => file.Status == FileEntryStatus.Ready || file is FileEntry { HasSuppliedContent: true, Status: FileEntryStatus.Error };

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
    /// Removes the file from the displayed collection and releases its browser data when present.
    /// </summary>
    /// <param name="file">The file entry to remove.</param>
    /// <param name="confirm">Wether to show a confirmation popup.</param>
    public Task RemoveFile( IFileEntry file, bool confirm = false )
    {
        var removeFileAction = async () =>
        {
            if ( !EffectiveFiles.Any( entry => ReferenceEquals( entry, file ) ) )
            {
                return;
            }

            if ( ReferenceEquals( file, fileBeingUploaded ) )
            {
                await Cancel();
            }

            if ( IsBrowserFile( file ) )
            {
                await FileInputRef.RemoveFiles( new[] { file } );
            }

            await UpdateFiles( EffectiveFiles.Where( entry => !ReferenceEquals( entry, file ) ).ToArray() );
        };

        if ( confirm )
        {
            return filePickerConfirmModalRef.OpenModal( _FilePickerConfirmModal.ConfirmOperation.RemoveFile, removeFileAction );
        }

        return removeFileAction.Invoke();
    }

    /// <summary>
    /// Clears all displayed files, including preloaded attachments, and resets the browser selection.
    /// </summary>
    /// <param name="confirm">Wether to show a confirmation popup.</param>
    public Task Clear( bool confirm = false )
    {
        var clearAction = async () =>
        {
            await Cancel();

            if ( FileInputRef?.Files is not null )
            {
                await FileInputRef.RemoveFiles( FileInputRef.Files );
            }

            await UpdateFiles( Array.Empty<IFileEntry>() );
        };

        if ( confirm )
        {
            return filePickerConfirmModalRef.OpenModal( _FilePickerConfirmModal.ConfirmOperation.Clear, clearAction );
        }

        return clearAction.Invoke();
    }

    /// <summary>
    /// Uploads the current files.
    /// </summary>
    /// <returns></returns>
    public async Task UploadAll()
    {
        if ( Upload.HasDelegate && EffectiveFiles.Length > 0 )
        {
            cts = new();
            foreach ( var file in EffectiveFiles.ToArray() )
            {
                if ( cts.IsCancellationRequested )
                {
                    break;
                }

                if ( !EffectiveFiles.Any( entry => ReferenceEquals( entry, file ) ) || !CanUploadFile( file ) )
                {
                    continue;
                }

                await UploadFile( file );

                if ( IsBrowserFile( file ) && file.Status == FileEntryStatus.Uploaded )
                {
                    await FileInputRef.RemoveFileEntry( file );
                }
            }
        }
    }

    /// <summary>
    /// Uploads a ready file or retries a cached file whose previous upload failed.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="forceUpload">Forces a file upload attempt regardless of status. The file must still have available content.</param>
    /// <returns></returns>
    public async Task UploadFile( IFileEntry file, bool forceUpload = false )
    {
        if ( !CanUploadFile( file ) )
        {
            return;
        }

        if ( cts is null || ( cts.IsCancellationRequested && !IsBusy() ) )
        {
            cts?.Dispose();
            cts = new();
        }

        if ( forceUpload || ( ShouldUploadFile( file ) && !cts.IsCancellationRequested ) )
        {
            if ( file is FileEntry { HasSuppliedContent: true } entry )
            {
                await UploadCachedFile( entry, cts.Token );
            }
            else
            {
                await Upload.InvokeAsync( new( file ) );
            }
        }
    }

    private async Task UploadCachedFile( FileEntry file, CancellationToken cancellationToken )
    {
        file.UploadNotifier = FileInputRef;
        file.UploadCancellationToken = cancellationToken;

        try
        {
            await FileInputRef.UpdateFileStartedAsync( file );
            cancellationToken.ThrowIfCancellationRequested();

            await Upload.InvokeAsync( new( file ) );
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch ( OperationCanceledException )
        {
            await FileInputRef.UpdateFileEndedAsync( file, false, FileInvalidReason.TaskCancelled );
            return;
        }
        catch
        {
            await FileInputRef.UpdateFileEndedAsync( file, false, FileInvalidReason.UnexpectedError );
            throw;
        }
        finally
        {
            file.UploadNotifier = null;
            file.UploadCancellationToken = default;
        }

        await FileInputRef.UpdateFileEndedAsync( file, true, FileInvalidReason.None );
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
    protected Task RemoveFileWithConfirm( IFileEntry file )
        => RemoveFile( file, true );

    /// <summary>
    /// Internal method. Facilitates named binding.
    /// </summary>
    /// <returns></returns>
    protected Task ClearWithConfirm()
        => Clear( true );

    /// <summary>
    /// Internal method. Facilitates named binding.
    /// </summary>
    /// <returns></returns>
    protected Task CancelWithConfirm()
        => Cancel( true );

    private CancellationTokenSource cts;

    /// <summary>
    /// FilePicker's handling of the Started Event.
    /// </summary>
    /// <param name="fileStartedEventArgs"></param>
    /// <returns></returns>
    protected async Task OnStarted( FileStartedEventArgs fileStartedEventArgs )
    {
        fileBeingUploaded = fileStartedEventArgs.File;
        await Started.InvokeAsync( fileStartedEventArgs );

        if ( ShouldReconcileFiles( EffectiveFiles ) )
        {
            ExecuteAfterRender( ReconcileFiles );
        }
    }

    /// <summary>
    /// FilePicker's handling of the Changed Event.
    /// </summary>
    /// <param name="fileChangedEventArgs"></param>
    /// <returns></returns>
    protected Task OnChanged( FileChangedEventArgs fileChangedEventArgs )
    {
        reconciledBrowserFiles = FileInputRef.Files ?? Array.Empty<IFileEntry>();

        var files = EffectiveFiles
            .Where( file => !IsBrowserFile( file ) )
            .Concat( reconciledBrowserFiles )
            .ToArray();

        return UpdateFiles( files );
    }

    private async Task UpdateFiles( IFileEntry[] files )
    {
        var filesChanged = !Enumerable.SequenceEqual<IFileEntry>( EffectiveFiles, files, ReferenceEqualityComparer.Instance );
        Files = files;

        if ( filesChanged )
        {
            await FilesChanged.InvokeAsync( files );
        }

        await Changed.InvokeAsync( new( files ) );

        if ( ShouldReconcileFiles( EffectiveFiles ) )
        {
            ExecuteAfterRender( ReconcileFiles );
        }

        await InvokeAsync( StateHasChanged );
    }

    private bool ShouldReconcileFiles( IFileEntry[] files )
    {
        if ( fileBeingUploaded is { Status: FileEntryStatus.Uploading }
            && !files.Any( file => ReferenceEquals( file, fileBeingUploaded ) ) )
        {
            return true;
        }

        return FileInputRef is not null
            && reconciledBrowserFiles.Any( file => !files.Any( entry => ReferenceEquals( entry, file ) ) );
    }

    private async Task ReconcileFiles()
    {
        if ( fileBeingUploaded is { Status: FileEntryStatus.Uploading }
            && !EffectiveFiles.Any( file => ReferenceEquals( file, fileBeingUploaded ) ) )
        {
            await Cancel();
        }

        if ( FileInputRef is null || reconciledBrowserFiles.Length == 0 )
        {
            return;
        }

        var removedFiles = reconciledBrowserFiles
            .Where( file => !EffectiveFiles.Any( entry => ReferenceEquals( entry, file ) ) )
            .ToArray();

        if ( removedFiles.Length == 0 )
        {
            return;
        }

        reconciledBrowserFiles = reconciledBrowserFiles
            .Where( file => !removedFiles.Any( entry => ReferenceEquals( entry, file ) ) )
            .ToArray();

        await FileInputRef.RemoveFiles( removedFiles );
    }

    /// <summary>
    /// FilePicker's handling of the Ended Event.
    /// </summary>
    /// <param name="fileEndedEventArgs"></param>
    /// <returns></returns>
    protected async Task OnEnded( FileEndedEventArgs fileEndedEventArgs )
    {
        fileBeingUploaded = null;

        if ( AutoReset && fileEndedEventArgs.Success
            && !IsBrowserFile( fileEndedEventArgs.File ) )
        {
            await UpdateFiles( EffectiveFiles.Where( file => !ReferenceEquals( file, fileEndedEventArgs.File ) ).ToArray() );
        }

        await Ended.InvokeAsync( fileEndedEventArgs );
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
    /// Accesses the FileInput
    /// </summary>
    public FileInput FileInputRef;

    /// <summary>
    /// Gets or sets the main container element id.
    /// </summary>
    protected string ElementContainerId { get; set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the files displayed by the picker, including preloaded attachments.
    /// </summary>
    protected IFileEntry[] EffectiveFiles => Files ?? Array.Empty<IFileEntry>();

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
    /// Defines the <see cref="IJSFilePickerModule"/> instance.
    /// </summary>
    [Inject] public IJSFilePickerModule JSFilePickerModule { get; set; }

    /// <summary>
    /// Defines the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Defines the DI registered <see cref="ITextLocalizer{FilePicker}"/>.
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
    /// Gets or sets the displayed files. Preloaded attachments can be supplied with
    /// <see cref="FileEntryStatus.Uploaded"/> status and do not require browser file data.
    /// Supply <see cref="FileEntry.Data"/> or <see cref="FileEntry.StreamFactory"/> and <see cref="FileEntryStatus.Ready"/> status to upload files restored from a local cache.
    /// Supplying file metadata alone does not make its contents available for streaming or upload.
    /// </summary>
    [Parameter] public IFileEntry[] Files { get; set; } = Array.Empty<IFileEntry>();

    /// <summary>
    /// Occurs when files are selected, removed, or cleared. Supports two-way binding of <see cref="Files"/>.
    /// </summary>
    [Parameter] public EventCallback<IFileEntry[]> FilesChanged { get; set; }

    /// <summary>
    /// Enables the multiple file selection.
    /// </summary>
    [Parameter] public bool Multiple { get; set; }

    /// <summary>
    /// Sets the placeholder for the empty file input.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// Specifies the types of files that the input accepts. See <see href="https://www.w3schools.com/tags/att_input_accept.asp">w3schools.com</see>.
    /// </summary>
    [Parameter] public string Filter { get; set; }

    /// <summary>
    /// Specifies the max chunk size when uploading the file.
    /// </summary>
    [Parameter] public int MaxChunkSize { get; set; } = 20 * 1024;

    /// <summary>
    /// Maximum file size in bytes, checked before starting upload (note: never trust client, always check file
    /// size at server-side). Defaults to <see cref="long.MaxValue"/>.
    /// </summary>
    [Parameter] public long MaxFileSize { get; set; } = long.MaxValue;

    /// <summary>
    /// Specifies the Segment Fetch Timeout when uploading the file.
    /// </summary>
    [Parameter] public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Occurs when the displayed collection changes through selection, removal, or reset.
    /// Includes preloaded attachments. Assigning <see cref="Files"/> from the parent does not raise this event.
    /// </summary>
    [Parameter] public EventCallback<FileChangedEventArgs> Changed { get; set; }

    /// <summary>
    /// Notifies when an individual file upload has started.
    /// </summary>
    [Parameter] public EventCallback<FileStartedEventArgs> Started { get; set; }

    /// <summary>
    /// Notifies when an individual file upload has ended.
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
    /// Specifies FilePicker's show mode.
    /// Defaults to <see cref="FilePickerShowMode.List"/>
    /// </summary>
    [Parameter] public FilePickerShowMode ShowMode { get; set; } = FilePickerShowMode.List;

    /// <summary>
    /// Determines whether report progress should be disabled. By enabling this setting, Progressed and Written callbacks won't be called. Internal file progress won't be tracked.
    /// <para>This setting can speed up file transfer considerably.</para>
    /// </summary>
    [Parameter] public bool DisableProgressReport { get; set; } = false;

    /// <summary>
    /// Determines whether file picker should upload directories.
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