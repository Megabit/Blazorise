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
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Builds upon FileEdit component providing extra file uploading features.
    /// </summary>
    public partial class FilePicker : BaseComponent
    {
        #region Members



        #endregion

        #region Methods


        #endregion

        #region Properties

        /// <summary>
        /// Accesses the FileEdit
        /// </summary>
        public FileEdit FileEdit;

        /// <summary>
        /// Input content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Enables the multiple file selection.
        /// </summary>
        [Parameter]
        public bool Multiple { get; set; }

        /// <summary>
        /// Sets the placeholder for the empty file input.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Specifies the types of files that the input accepts. https://www.w3schools.com/tags/att_input_accept.asp"
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
