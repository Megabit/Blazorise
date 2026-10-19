#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Docs.Docs.Examples;
using Blazorise.Modules;
using Blazorise.Tests.TestServices;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class FilePickerComponentTest : BunitContext
{
    #region Constructors

    public FilePickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        Services.AddScoped<IJSFilePickerModule, JSFilePickerModule>();

        JSInterop.AddBlazoriseButton();
        JSInterop.AddBlazoriseModal();
        JSInterop.AddBlazoriseDropdown();

        var inputModule = JSInterop.SetupModule( "./_content/Blazorise/fileInput.js?v=" );
        inputModule.SetupVoid( "initialize", _ => true ).SetVoidResult();
        inputModule.SetupVoid( "destroy", _ => true ).SetVoidResult();
        inputModule.SetupVoid( "removeFiles", _ => true ).SetVoidResult();

        var pickerModule = JSInterop.SetupModule( "./_content/Blazorise/filePicker.js?v=" );
        pickerModule.SetupVoid( "initialize", _ => true ).SetVoidResult();
        pickerModule.SetupVoid( "destroy", _ => true ).SetVoidResult();

        var fileModule = JSInterop.SetupModule( "./_content/Blazorise/io.js?v=" );
        fileModule.SetupVoid( "removeFileEntry", _ => true ).SetVoidResult();
    }

    #endregion

    #region Methods

    [Fact]
    public async Task BindingUpdatesTheParentAndAcceptsParentDrivenClear()
    {
        var comp = Render<FilePickerBindingExample>();
        var picker = comp.FindComponent<FilePicker>();

        await comp.InvokeAsync( () => picker.Instance.FileInputRef.NotifyChange( new[] { new FileEntry { Id = 1, Name = "selected.txt" } } ) );

        Assert.Contains( "Attachments: 2", comp.Markup );

        comp.FindAll( "button" ).Single( button => button.TextContent == "Clear bound files" ).Click();

        comp.WaitForAssertion( () => Assert.Empty( picker.Instance.FileInputRef.Files ) );
        Assert.Empty( picker.Instance.Files );
        Assert.Contains( "Attachments: 0", comp.Markup );
    }

    [Theory]
    [InlineData( FilePickerShowMode.List )]
    [InlineData( FilePickerShowMode.Dropdown )]
    public void DisplaysPreloadedEntriesWithDuplicateIdsWithoutNotifyingParent( FilePickerShowMode showMode )
    {
        var first = new FileEntry { Name = "first.pdf", Status = FileEntryStatus.Uploaded };
        var second = new FileEntry { Name = "second.pdf", Status = FileEntryStatus.Uploaded };
        var notifications = 0;
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { first, second } )
            .Add( x => x.ShowMode, showMode )
            .Add( x => x.FilesChanged, _ => notifications++ )
            .Add( x => x.Changed, _ => notifications++ ) );

        Assert.Equal( 2, comp.FindAll( ".b-file-picker-file-name" ).Count );
        Assert.False( comp.Instance.IsUploadReady() );
        Assert.Equal( 0, notifications );

        comp.Render( parameters => parameters.Add( x => x.Files, new IFileEntry[] { second } ) );

        Assert.Equal( "second.pdf", comp.Find( ".b-file-picker-file-name" ).TextContent );
        Assert.Equal( 0, notifications );
    }

    [Fact]
    public async Task PreservesPreloadedEntriesAndCanonicalBrowserInstancesAcrossSelections()
    {
        var existing = new FileEntry { Id = 1, Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var first = new FileEntry { Id = 1, Name = "first.txt" };
        var second = new FileEntry { Id = 2, Name = "second.txt" };
        var notifications = new List<IFileEntry[]>();
        IFileEntry[] changedFiles = null;
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { existing } )
            .Add( x => x.FilesChanged, files => notifications.Add( files ) )
            .Add( x => x.Changed, args => changedFiles = args.Files ) );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { first } ) );

        Assert.Equal( new IFileEntry[] { existing, first }, comp.Instance.Files );
        Assert.Same( comp.Instance.Files, changedFiles );
        Assert.Same( comp.Instance.FileInputRef, first.Owner );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { new FileEntry { Id = 1, Name = "first.txt" } } ) );

        Assert.Same( first, comp.Instance.Files[1] );
        Assert.Single( notifications );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { second } ) );

        Assert.Equal( new IFileEntry[] { existing, second }, comp.Instance.Files );
        Assert.Equal( 2, notifications.Count );
        Assert.Throws<InvalidOperationException>( () => first.OpenReadStream() );
    }

    [Fact]
    public async Task RemovesMetadataAndBrowserFilesIndependentlyWhenIdsCollide()
    {
        var existing = new FileEntry { Id = 1, Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var notifications = new List<IFileEntry[]>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { existing } )
            .Add( x => x.FilesChanged, files => notifications.Add( files ) ) );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );
        await comp.InvokeAsync( () => comp.Instance.RemoveFile( existing ) );

        Assert.Same( selected, Assert.Single( comp.Instance.Files ) );
        Assert.Same( selected, Assert.Single( comp.Instance.FileInputRef.Files ) );
        Assert.Empty( JSInterop.Invocations["removeFiles"] );

        await comp.InvokeAsync( () => comp.Instance.RemoveFile( selected ) );

        Assert.Empty( comp.Instance.Files );
        Assert.Empty( comp.Instance.FileInputRef.Files );
        Assert.Equal( new[] { 1 }, Assert.IsType<int[]>( Assert.Single( JSInterop.Invocations["removeFiles"] ).Arguments[2] ) );
        Assert.Equal( 3, notifications.Count );
        Assert.Throws<InvalidOperationException>( () => selected.OpenReadStream() );
    }

    [Fact]
    public async Task ParentRemovalReleasesBrowserSelectionWithoutEchoingCallbacks()
    {
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var notifications = 0;
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.FilesChanged, _ => notifications++ )
            .Add( x => x.Changed, _ => notifications++ ) );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );
        Assert.Equal( 2, notifications );

        comp.Render( parameters => parameters.Add( x => x.Files, (IFileEntry[])null ) );

        comp.WaitForAssertion( () => Assert.Empty( comp.Instance.FileInputRef.Files ) );
        Assert.Single( JSInterop.Invocations["removeFiles"] );
        Assert.Equal( 2, notifications );
        Assert.Throws<InvalidOperationException>( () => selected.OpenReadStream() );
    }

    [Fact]
    public async Task UnrelatedParameterUpdatesDoNotReconcileBrowserFiles()
    {
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var existing = new FileEntry { Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var comp = Render<FilePicker>();

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );

        var files = comp.Instance.Files;
        files[0] = existing;

        await comp.InvokeAsync( () => comp.Instance.SetParametersAsync( ParameterView.FromDictionary( new Dictionary<string, object>
        {
            [nameof( FilePicker.Disabled )] = true,
        } ) ) );

        Assert.Same( selected, Assert.Single( comp.Instance.FileInputRef.Files ) );
        Assert.Empty( JSInterop.Invocations["removeFiles"] );

        comp.Render( parameters => parameters.Add( x => x.Files, files ) );

        comp.WaitForAssertion( () => Assert.Empty( comp.Instance.FileInputRef.Files ) );
        Assert.Single( JSInterop.Invocations["removeFiles"] );
        Assert.Same( existing, Assert.Single( comp.Instance.Files ) );
    }

    [Fact]
    public async Task RenderDuringValidationDoesNotDiscardAnIncomingSelection()
    {
        var continueValidation = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        var validationStarted = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var comp = Render<Validation>( parameters => parameters
            .Add( x => x.AsyncValidator, async ( args, cancellationToken ) =>
            {
                if ( args.Value is IFileEntry[] { Length: > 0 } )
                {
                    validationStarted.TrySetResult();
                    await continueValidation.Task;
                }

                args.Status = ValidationStatus.Success;
            } )
            .AddChildContent<FilePicker>( inputParameters => inputParameters.Add( x => x.Multiple, true ) ) );

        var picker = comp.FindComponent<FilePicker>();
        var selectionTask = comp.InvokeAsync( () => picker.Instance.FileInputRef.NotifyChange( new[] { selected } ) );

        try
        {
            comp.WaitForAssertion( () => Assert.True( validationStarted.Task.IsCompleted ) );
            picker.Render( parameters => parameters.Add( x => x.Files, Array.Empty<IFileEntry>() ) );

            Assert.Same( selected, Assert.Single( picker.Instance.FileInputRef.Files ) );
            Assert.Empty( JSInterop.Invocations["removeFiles"] );
        }
        finally
        {
            continueValidation.TrySetResult();
        }

        await selectionTask;

        Assert.Same( selected, Assert.Single( picker.Instance.Files ) );
        Assert.True( picker.Instance.IsUploadReady() );
    }

    [Fact]
    public async Task BrowserResetPreservesMetadataAndClearRemovesAllEntries()
    {
        var existing = new FileEntry { Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var notifications = new List<IFileEntry[]>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { existing } )
            .Add( x => x.FilesChanged, files => notifications.Add( files ) ) );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );
        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( Array.Empty<FileEntry>() ) );

        Assert.Same( existing, Assert.Single( comp.Instance.Files ) );
        Assert.Throws<InvalidOperationException>( () => selected.OpenReadStream() );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { new FileEntry { Id = 2, Name = "another.txt" } } ) );
        await comp.InvokeAsync( () => comp.Instance.Clear() );

        Assert.Empty( comp.Instance.Files );
        Assert.Empty( comp.Instance.FileInputRef.Files );
        Assert.Empty( notifications[3] );
        Assert.Single( JSInterop.Invocations["removeFiles"] );
    }

    [Fact]
    public async Task UploadsOnlyEntriesWithContentAndPreventsForcedUploadAfterRelease()
    {
        var metadata = new FileEntry { Name = "metadata.txt" };
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var uploads = new List<IFileEntry>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { metadata } )
            .Add( x => x.Upload, args =>
            {
                uploads.Add( args.File );
                args.File.Status = FileEntryStatus.Uploaded;
            } ) );

        Assert.False( comp.Instance.IsUploadReady() );
        Assert.Single( comp.FindAll( ".b-file-picker-file button" ) );

        await comp.InvokeAsync( () => comp.Instance.UploadFile( metadata, true ) );
        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );

        Assert.True( comp.Instance.IsUploadReady() );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );
        await comp.InvokeAsync( () => comp.Instance.UploadFile( selected, true ) );

        Assert.Same( selected, Assert.Single( uploads ) );
        Assert.Single( JSInterop.Invocations["removeFileEntry"] );
        Assert.Equal( 2, comp.Instance.Files.Length );
        Assert.False( comp.Instance.IsUploadReady() );
    }

    [Fact]
    public async Task MetadataStreamingFailsWithAnExplanation()
    {
        var file = new FileEntry { Name = "existing.pdf", UploadUrl = "/attachments/existing.pdf" };

        var readException = Assert.Throws<InvalidOperationException>( () => file.OpenReadStream() );
        var writeException = await Assert.ThrowsAsync<InvalidOperationException>( () => file.WriteToStreamAsync( Stream.Null ) );

        Assert.Contains( "no available content", readException.Message );
        Assert.Contains( "no available content", writeException.Message );
    }

    [Theory]
    [InlineData( false, false, false )]
    [InlineData( true, false, false )]
    [InlineData( false, true, false )]
    [InlineData( true, true, false )]
    [InlineData( false, false, true )]
    [InlineData( true, false, true )]
    [InlineData( false, true, true )]
    [InlineData( true, true, true )]
    public async Task UploadsRestoredContentWithTheNormalLifecycle( bool writeToStream, bool disableProgressReport, bool useStreamFactory )
    {
        var original = new FileEntry
        {
            Name = "cached.bin",
            Data = new byte[] { 1, 2, 3, 4, 5 },
            Status = FileEntryStatus.Ready,
        };

        var restored = JsonSerializer.Deserialize<FileEntry>( JsonSerializer.Serialize( original ) );
        var openedSources = new List<Stream>();

        if ( useStreamFactory )
        {
            restored.Data = null;
            restored.StreamFactory = _ =>
            {
                var source = new MemoryStream( original.Data, writable: false );
                openedSources.Add( source );
                return Task.FromResult<Stream>( source );
            };
        }

        var started = new List<IFileEntry>();
        var ended = new List<FileEndedEventArgs>();
        var written = new List<byte>();
        var progress = new List<double>();
        await using var destination = new MemoryStream();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { restored } )
            .Add( x => x.MaxChunkSize, 2 )
            .Add( x => x.DisableProgressReport, disableProgressReport )
            .Add( x => x.Started, args => started.Add( args.File ) )
            .Add( x => x.Ended, args => ended.Add( args ) )
            .Add( x => x.Written, args => written.AddRange( args.Data ) )
            .Add( x => x.Progressed, args => progress.Add( args.Progress ) )
            .Add( x => x.Upload, async args =>
            {
                Assert.Equal( FileEntryStatus.Uploading, args.File.Status );

                if ( writeToStream )
                {
                    await args.File.WriteToStreamAsync( destination );
                }
                else
                {
                    await using var stream = args.File.OpenReadStream();
                    var buffer = new byte[2];
                    var bytesRead = 0;

                    while ( ( bytesRead = await stream.ReadAsync( buffer.AsMemory() ) ) > 0 )
                    {
                        await destination.WriteAsync( buffer.AsMemory( 0, bytesRead ) );
                    }
                }
            } ) );

        Assert.True( comp.Instance.IsUploadReady() );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );

        Assert.Equal( original.Data, destination.ToArray() );
        Assert.Same( restored, Assert.Single( started ) );
        Assert.True( Assert.Single( ended ).Success );
        Assert.Equal( FileEntryStatus.Uploaded, restored.Status );
        Assert.False( comp.Instance.IsBusy() );
        Assert.Null( restored.Owner );
        Assert.Empty( JSInterop.Invocations["removeFileEntry"] );

        if ( useStreamFactory )
        {
            Assert.Null( restored.Data );
            Assert.False( Assert.Single( openedSources ).CanRead );
        }
        else
        {
            Assert.Equal( original.Data, restored.Data );
        }

        if ( disableProgressReport )
        {
            Assert.Empty( written );
            Assert.Empty( progress );
        }
        else
        {
            Assert.Equal( original.Data, written );
            Assert.Equal( 1d, progress.Last() );
        }
    }

    [Theory]
    [InlineData( 0, false )]
    [InlineData( 3, false )]
    [InlineData( 0, true )]
    [InlineData( 3, true )]
    public async Task AutoResetRemovesOnlyTheUploadedCachedEntry( int length, bool useStreamFactory )
    {
        var existing = new FileEntry { Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var first = CreateCachedFile( "first.bin", new byte[length], useStreamFactory );
        var second = CreateCachedFile( "second.bin", new byte[length], useStreamFactory );
        var uploads = new List<IFileEntry>();
        var progress = new List<double>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { existing, first, second } )
            .Add( x => x.AutoReset, true )
            .Add( x => x.Progressed, args => progress.Add( args.Progress ) )
            .Add( x => x.Upload, async args =>
            {
                uploads.Add( args.File );
                await args.File.WriteToStreamAsync( Stream.Null );
            } ) );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );

        Assert.Equal( new IFileEntry[] { first, second }, uploads );
        Assert.Same( existing, Assert.Single( comp.Instance.Files ) );
        Assert.All( progress, value => Assert.Equal( 1d, value ) );
        Assert.Equal( 2, progress.Count );
        Assert.Empty( JSInterop.Invocations["removeFiles"] );
        Assert.True( first.Data is not null || first.StreamFactory is not null );
        Assert.True( second.Data is not null || second.StreamFactory is not null );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task CachedUploadFailureRetainsTheContentSourceForRetry( bool useStreamFactory )
    {
        var file = CreateCachedFile( "cached.bin", new byte[] { 1, 2, 3 }, useStreamFactory );
        var shouldFail = true;
        var ended = new List<FileEndedEventArgs>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.AutoReset, true )
            .Add( x => x.Ended, args => ended.Add( args ) )
            .Add( x => x.Upload, async args =>
            {
                await args.File.WriteToStreamAsync( Stream.Null );

                if ( shouldFail )
                {
                    throw new IOException( "Upload failed." );
                }
            } ) );

        await Assert.ThrowsAsync<IOException>( () => comp.InvokeAsync( () => comp.Instance.UploadAll() ) );

        Assert.Equal( FileEntryStatus.Error, file.Status );
        Assert.Same( file, Assert.Single( comp.Instance.Files ) );
        Assert.False( comp.Instance.IsBusy() );
        Assert.Equal( FileInvalidReason.UnexpectedError, Assert.Single( ended ).FileInvalidReason );

        Assert.True( comp.Instance.IsUploadReady() );

        shouldFail = false;
        await comp.InvokeAsync( () => comp.Instance.UploadFile( file ) );

        Assert.Empty( comp.Instance.Files );
        Assert.Equal( FileEntryStatus.Uploaded, file.Status );
        Assert.True( ended[1].Success );
        Assert.True( file.Data is not null || file.StreamFactory is not null );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task CancelledCachedUploadsCanBeRetried( bool useStreamFactory )
    {
        var file = CreateCachedFile( "cached.bin", new byte[] { 1, 2, 3 }, useStreamFactory );
        var shouldCancel = true;
        var ended = new List<FileEndedEventArgs>();
        IRenderedComponent<FilePicker> comp = null;
        comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.MaxChunkSize, 1 )
            .Add( x => x.AutoReset, true )
            .Add( x => x.Ended, args => ended.Add( args ) )
            .Add( x => x.Written, async _ =>
            {
                if ( shouldCancel )
                {
                    await comp.Instance.Cancel();
                }
            } )
            .Add( x => x.Upload, args => args.File.WriteToStreamAsync( Stream.Null ) ) );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );

        Assert.Equal( FileEntryStatus.Error, file.Status );
        Assert.Equal( FileInvalidReason.TaskCancelled, Assert.Single( ended ).FileInvalidReason );
        Assert.Same( file, Assert.Single( comp.Instance.Files ) );
        Assert.False( comp.Instance.IsBusy() );

        shouldCancel = false;
        await comp.InvokeAsync( () => comp.Instance.UploadFile( file ) );

        Assert.Empty( comp.Instance.Files );
        Assert.Equal( FileEntryStatus.Uploaded, file.Status );
        Assert.True( ended[1].Success );
    }

    [Fact]
    public async Task CachedEntriesUseTheirActualSizeForThePickerLimit()
    {
        var file = new FileEntry
        {
            Name = "cached.bin",
            Size = 1,
            Data = new byte[] { 1, 2, 3, 4 },
        };

        var uploads = 0;
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.MaxFileSize, 3 )
            .Add( x => x.Upload, async args =>
            {
                uploads++;
                await args.File.WriteToStreamAsync( Stream.Null );
            } ) );

        Assert.Equal( FileEntryStatus.ExceedsMaximumSize, file.Status );
        Assert.False( comp.Instance.IsUploadReady() );

        await comp.InvokeAsync( () => comp.Instance.UploadFile( file, true ) );

        Assert.Equal( 0, uploads );

        comp.Render( parameters => parameters.Add( x => x.MaxFileSize, 4 ) );

        Assert.Equal( FileEntryStatus.Ready, file.Status );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );

        Assert.Equal( 1, uploads );
        Assert.Equal( FileEntryStatus.Uploaded, file.Status );
    }

    [Fact]
    public void ResupplyingTheSameFilesRefreshesStatusesAfterContentChanges()
    {
        var file = new FileEntry { Name = "cached.bin", Data = new byte[2] };
        var files = new IFileEntry[] { file };
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, files )
            .Add( x => x.MaxFileSize, 3 ) );

        Assert.Equal( FileEntryStatus.Ready, file.Status );

        file.Data = new byte[4];
        comp.Render( parameters => parameters.Add( x => x.Files, files ) );

        Assert.Equal( FileEntryStatus.ExceedsMaximumSize, file.Status );
        Assert.Equal( "File size is too large", comp.Find( ".b-file-picker-file-status" ).TextContent );

        file.Data = new byte[2];
        comp.Render( parameters => parameters.Add( x => x.Files, files ) );

        Assert.Equal( FileEntryStatus.Ready, file.Status );
        Assert.Equal( "Ready to upload", comp.Find( ".b-file-picker-file-status" ).TextContent );
    }

    [Fact]
    public async Task PartialParameterUpdatesRefreshStatusesOnlyWhenTheSizeLimitChanges()
    {
        var file = new FileEntry { Name = "cached.bin", Data = new byte[2] };
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.MaxFileSize, 3 ) );

        file.Data = new byte[4];

        // Deliver only these parameters without resupplying the mutable Files collection.
        await comp.InvokeAsync( () => comp.Instance.SetParametersAsync( ParameterView.FromDictionary( new Dictionary<string, object>
        {
            [nameof( FilePicker.Disabled )] = true,
            [nameof( FilePicker.MaxFileSize )] = 3L,
        } ) ) );

        Assert.Equal( FileEntryStatus.Ready, file.Status );

        await comp.InvokeAsync( () => comp.Instance.SetParametersAsync( ParameterView.FromDictionary( new Dictionary<string, object>
        {
            [nameof( FilePicker.MaxFileSize )] = 2L,
        } ) ) );

        Assert.Equal( FileEntryStatus.ExceedsMaximumSize, file.Status );
        Assert.Equal( "File size is too large", comp.Find( ".b-file-picker-file-status" ).TextContent );
    }

    [Fact]
    public async Task CachedUploadsPreservePendingBrowserSelectionsAndMetadata()
    {
        var existing = new FileEntry { Id = 1, Name = "existing.pdf", Status = FileEntryStatus.Uploaded };
        var cached = new FileEntry { Id = 1, Name = "cached.bin", Data = new byte[] { 1, 2, 3 } };
        var selected = new FileEntry { Id = 1, Name = "selected.txt" };
        var replacement = new FileEntry { Id = 2, Name = "replacement.txt" };
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { existing, cached } )
            .Add( x => x.AutoReset, true )
            .Add( x => x.Upload, async args =>
            {
                await args.File.WriteToStreamAsync( Stream.Null );
                cached.Data = null;
            } ) );

        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { selected } ) );
        await comp.InvokeAsync( () => comp.Instance.FileInputRef.NotifyChange( new[] { replacement } ) );

        Assert.Equal( new IFileEntry[] { existing, cached, replacement }, comp.Instance.Files );

        await comp.InvokeAsync( () => comp.Instance.UploadFile( cached ) );

        Assert.Equal( new IFileEntry[] { existing, replacement }, comp.Instance.Files );
        Assert.Same( replacement, Assert.Single( comp.Instance.FileInputRef.Files ) );
        Assert.True( comp.Instance.IsUploadReady() );
        Assert.Empty( JSInterop.Invocations["removeFiles"] );
        Assert.Empty( JSInterop.Invocations["removeFileEntry"] );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task ParentRemovalCancelsAnActiveCachedUploadWithoutDiscardingItsContentSource( bool useStreamFactory )
    {
        var file = CreateCachedFile( "cached.bin", new byte[] { 1, 2, 3 }, useStreamFactory );
        var uploadStarted = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        var continueUpload = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        var ended = new List<FileEndedEventArgs>();
        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.Ended, args => ended.Add( args ) )
            .Add( x => x.Upload, async args =>
            {
                uploadStarted.TrySetResult();
                await continueUpload.Task;
                await args.File.WriteToStreamAsync( Stream.Null );
            } ) );

        var uploadTask = comp.InvokeAsync( () => comp.Instance.UploadAll() );

        try
        {
            comp.WaitForAssertion( () => Assert.True( uploadStarted.Task.IsCompleted ) );
            comp.Render( parameters => parameters.Add( x => x.Files, Array.Empty<IFileEntry>() ) );
        }
        finally
        {
            continueUpload.TrySetResult();
        }

        await uploadTask;

        Assert.Empty( comp.Instance.Files );
        Assert.False( comp.Instance.IsBusy() );
        Assert.Equal( FileInvalidReason.TaskCancelled, Assert.Single( ended ).FileInvalidReason );
        Assert.True( file.Data is not null || file.StreamFactory is not null );
    }

    [Fact]
    public async Task StreamedUploadsRespectThePickerChunkSizeWithoutPopulatingData()
    {
        var length = 1024 * 1024;
        var source = new GeneratedReadStream( length );
        var opens = 0;
        var file = new FileEntry
        {
            Name = "large.bin",
            Size = length,
            StreamFactory = _ =>
            {
                opens++;
                return Task.FromResult<Stream>( source );
            },
        };

        var comp = Render<FilePicker>( parameters => parameters
            .Add( x => x.Files, new IFileEntry[] { file } )
            .Add( x => x.MaxChunkSize, 4096 )
            .Add( x => x.Upload, args => args.File.WriteToStreamAsync( Stream.Null ) ) );

        Assert.Equal( 0, opens );

        await comp.InvokeAsync( () => comp.Instance.UploadAll() );

        Assert.Equal( 1, opens );
        Assert.Equal( length, source.BytesRead );
        Assert.InRange( source.MaxRequestedSize, 1, 4096 );
        Assert.True( source.IsDisposedAsynchronously );
        Assert.Null( file.Data );
        Assert.Equal( FileEntryStatus.Uploaded, file.Status );
        Assert.Equal( 100, comp.Instance.GetProgressPercentage() );
    }

    private static FileEntry CreateCachedFile( string name, byte[] data, bool useStreamFactory )
    {
        var file = new FileEntry
        {
            Name = name,
            Size = data.LongLength,
        };

        if ( useStreamFactory )
        {
            file.StreamFactory = _ => Task.FromResult<Stream>( new MemoryStream( data, writable: false ) );
        }
        else
        {
            file.Data = data;
        }

        return file;
    }

    #endregion
}