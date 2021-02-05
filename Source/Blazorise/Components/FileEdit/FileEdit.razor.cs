#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </summary>
    public interface IFileEdit
    {
        Task NotifyChange( FileEntry[] files );
    }

    /// <summary>
    /// Input component with support for single of multi file upload.
    /// </summary>
    public partial class FileEdit : BaseInputComponent<IFileEntry[]>, IFileEdit
    {
        #region Members

        private bool multiple;

        // taken from https://github.com/aspnet/AspNetCore/issues/11159
        private DotNetObjectReference<FileEditAdapter> dotNetObjectRef;

        private IFileEntry[] files;

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                InitializeValidation();
            }
        }

        protected override void OnInitialized()
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        private async void OnLocalizationChanged( object sender, EventArgs e )
        {
            // no need to refresh if we're using custom localization
            if ( BrowseButtonLocalizer != null )
                return;

            await InvokeAsync( StateHasChanged );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FileEdit() );
            builder.Append( ClassProvider.FileEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new FileEditAdapter( this ) );

            await JSRunner.InitializeFileEdit( dotNetObjectRef, ElementRef, ElementId );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                JSRunner.DestroyFileEdit( ElementRef, ElementId );
                DisposeDotNetObjectRef( dotNetObjectRef );

                LocalizerService.LocalizationChanged -= OnLocalizationChanged;
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Notifies the component that file input value has changed.
        /// </summary>
        /// <param name="files">List of changed file(s).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task NotifyChange( FileEntry[] files )
        {
            // Unlike in other Edit components we cannot just call CurrentValueHandler since
            // we're dealing with complex types instead of a simple string as en element value.
            //
            // Because of that we're going to skip CurrentValueHandler and implement all the
            // update logic here.

            foreach ( var file in files )
            {
                // So that method invocations on the file can be dispatched back here
                file.Owner = (FileEdit)(object)this;
            }

            InternalValue = files;

            // send the value to the validation for processing
            ParentValidation?.NotifyInputChanged<IFileEntry[]>( default );

            await Changed.InvokeAsync( new FileChangedEventArgs( files ) );

            await InvokeAsync( StateHasChanged );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( IFileEntry[] value )
        {
            throw new NotImplementedException( $"{nameof( OnInternalValueChanged )} in {nameof( FileEdit )} should never be called." );
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<IFileEntry[]>> ParseValueFromStringAsync( string value )
        {
            throw new NotImplementedException( $"{nameof( ParseValueFromStringAsync )} in {nameof( FileEdit )} should never be called." );
        }

        /// <summary>
        /// Notifies the component that file upload is about to start.
        /// </summary>
        /// <param name="fileEntry">File entry to be uploaded.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal Task UpdateFileStartedAsync( IFileEntry fileEntry )
        {
            // reset all
            ProgressProgress = 0;
            ProgressTotal = fileEntry.Size;
            Progress = 0;

            return Started.InvokeAsync( new FileStartedEventArgs( fileEntry ) );
        }

        /// <summary>
        /// Notifies the component that file upload has ended.
        /// </summary>
        /// <param name="fileEntry">Uploaded file entry.</param>
        /// <param name="success">True if the file upload was successful.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal async Task UpdateFileEndedAsync( IFileEntry fileEntry, bool success )
        {
            if ( AutoReset )
            {
                await Reset();
            }

            await Ended.InvokeAsync( new FileEndedEventArgs( fileEntry, success ) );
        }

        /// <summary>
        /// Updates component with the latest file data.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="position">The current position of this stream.</param>
        /// <param name="data">Curerntly read data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal Task UpdateFileWrittenAsync( IFileEntry fileEntry, long position, byte[] data )
        {
            return Written.InvokeAsync( new FileWrittenEventArgs( fileEntry, position, data ) );
        }

        /// <summary>
        /// Updated the component with the latest upload progress.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="progressProgress">Progress value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal async Task UpdateFileProgressAsync( IFileEntry fileEntry, long progressProgress )
        {
            ProgressProgress += progressProgress;

            var progress = Math.Round( (double)ProgressProgress / ProgressTotal, 3 );

            if ( Math.Abs( progress - Progress ) > double.Epsilon )
            {
                Progress = progress;

                await Progressed.InvokeAsync( new FileProgressedEventArgs( fileEntry, Progress ) );
            }
        }

        /// <summary>
        /// Writes the file data to the target stream.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="stream">Target stream.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal async Task WriteToStreamAsync( FileEntry fileEntry, Stream stream )
        {
            await new RemoteFileEntryStreamReader( JSRunner, ElementRef, fileEntry, this, MaxMessageSize )
                .WriteToStreamAsync( stream, CancellationToken.None );
        }

        /// <summary>
        /// Opens the stream for reading the uploaded file.
        /// </summary>
        /// <param name="fileEntry">Currently processed file entry.</param>
        /// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
        /// <returns>Returns the stream for the uploaded file entry.</returns>
        public Stream OpenReadStream( FileEntry fileEntry, CancellationToken cancellationToken )
        {
            return new RemoteFileEntryStream( JSRunner, ElementRef, fileEntry, this, MaxMessageSize, SegmentFetchTimeout, cancellationToken );
        }

        /// <summary>
        /// Manaully resets the input file value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Reset()
        {
            await JSRunner.ResetFileEdit( ElementRef, ElementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <inheritdoc/>
        protected override IFileEntry[] InternalValue { get => files; set => files = value; }

        protected long ProgressProgress;

        protected long ProgressTotal;

        protected double Progress;

        [Inject] protected ITextLocalizerService LocalizerService { get; set; }

        [Inject] protected ITextLocalizer<FileEdit> Localizer { get; set; }

        /// <summary>
        /// Gets the localized browse button text.
        /// </summary>
        protected string BrowseButtonString
        {
            get
            {
                var localizationString = Multiple
                    ? "Choose files"
                    : "Choose file";

                if ( BrowseButtonLocalizer != null )
                    return BrowseButtonLocalizer.Invoke( localizationString );

                return Localizer[localizationString];
            }
        }

        /// <summary>
        /// Gets the list is selected filename
        /// </summary>
        protected IEnumerable<string> SelectedFileNames => InternalValue?.Select( x => x.Name ) ?? Enumerable.Empty<string>();

        /// <summary>
        /// Enables the multiple file selection.
        /// </summary>
        [Parameter]
        public bool Multiple
        {
            get => multiple;
            set
            {
                multiple = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the types of files that the input accepts.
        /// </summary>
        /// <see cref="https://www.w3schools.com/tags/att_input_accept.asp"/>
        [Parameter] public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the max message size when uploading the file.
        /// </summary>
        [Parameter] public int MaxMessageSize { get; set; } = 20 * 1024;

        /// <summary>
        /// Gets or sets the Segment Fetch Timeout when uploading the file.
        /// </summary>
        [Parameter] public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

        /// <summary>
        /// Occurs every time the selected file(s) has changed.
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
        /// If true file input will be automatically reset after it has being uploaded.
        /// </summary>
        [Parameter] public bool AutoReset { get; set; } = true;

        /// <summary>
        /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
        /// </summary>
        [Parameter] public TextLocalizerHandler BrowseButtonLocalizer { get; set; }

        #endregion
    }
}
