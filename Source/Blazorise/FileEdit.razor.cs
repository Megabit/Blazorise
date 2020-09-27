﻿#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public partial class FileEdit : BaseInputComponent<IFileEntry[]>, IFileEdit
    {
        #region Members

        private bool multiple;

        // taken from https://github.com/aspnet/AspNetCore/issues/11159
        private DotNetObjectReference<FileEditAdapter> dotNetObjectRef;

        private IFileEntry[] files;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FileEdit() );
            builder.Append( ClassProvider.FileEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new FileEditAdapter( this ) );

            await JSRunner.InitializeFileEdit( dotNetObjectRef, ElementRef, ElementId );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                JSRunner.DestroyFileEdit( ElementRef, ElementId );
                JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
            }

            base.Dispose( disposing );
        }

        public Task NotifyChange( FileEntry[] files )
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
            ParentValidation?.NotifyInputChanged();

            return Changed.InvokeAsync( new FileChangedEventArgs( files ) );
        }

        protected override Task OnInternalValueChanged( IFileEntry[] value )
        {
            throw new NotImplementedException( $"{nameof( OnInternalValueChanged )} in {nameof( FileEdit )} should never be called." );
        }

        protected override Task<ParseValue<IFileEntry[]>> ParseValueFromStringAsync( string value )
        {
            throw new NotImplementedException( $"{nameof( ParseValueFromStringAsync )} in {nameof( FileEdit )} should never be called." );
        }

        internal Task UpdateFileStartedAsync( IFileEntry fileEntry )
        {
            // reset all
            ProgressProgress = 0;
            ProgressTotal = fileEntry.Size;
            Progress = 0;

            return Started.InvokeAsync( new FileStartedEventArgs( fileEntry ) );
        }

        internal Task UpdateFileEndedAsync( IFileEntry fileEntry, bool success )
        {
            return Ended.InvokeAsync( new FileEndedEventArgs( fileEntry, success ) );
        }

        internal Task UpdateFileWrittenAsync( IFileEntry fileEntry, long position, byte[] data )
        {
            return Written.InvokeAsync( new FileWrittenEventArgs( fileEntry, position, data ) );
        }

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

        internal async Task WriteToStreamAsync( FileEntry fileEntry, Stream stream )
        {
            await new RemoteFileEntryStreamReader( JSRunner, ElementRef, fileEntry, this, MaxMessageSize )
                .WriteToStreamAsync( stream, CancellationToken.None );
        }

        #endregion

        #region Properties

        protected override IFileEntry[] InternalValue { get => files; set => files = value; }

        protected long ProgressProgress;

        protected long ProgressTotal;

        protected double Progress;

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

        #endregion
    }
}
