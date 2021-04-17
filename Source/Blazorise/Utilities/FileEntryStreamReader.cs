#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class FileEntryStreamReader
    {
        protected readonly IJSRunner jsRunner;
        protected readonly ElementReference elementRef;
        protected readonly FileEntry fileEntry;
        protected readonly FileEdit fileEdit;

        public FileEntryStreamReader( IJSRunner jsRunner, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit )
        {
            this.jsRunner = jsRunner;
            this.elementRef = elementRef;
            this.fileEntry = fileEntry;
            this.fileEdit = fileEdit;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
