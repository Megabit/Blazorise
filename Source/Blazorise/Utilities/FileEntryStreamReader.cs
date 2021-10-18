#region Using directives
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class FileEntryStreamReader
    {
        private readonly IJSFileEditModule jsModule;
        private readonly ElementReference elementRef;
        private readonly FileEntry fileEntry;
        private readonly FileEdit fileEdit;

        public FileEntryStreamReader( IJSFileEditModule jsModule, ElementReference elementRef, FileEntry fileEntry, FileEdit fileEdit )
        {
            this.jsModule = jsModule;
            this.elementRef = elementRef;
            this.fileEntry = fileEntry;
            this.fileEdit = fileEdit;
        }

        protected IJSFileEditModule JSModule => jsModule;

        protected ElementReference ElementRef => elementRef;

        protected FileEntry FileEntry => fileEntry;

        protected FileEdit FileEdit => fileEdit;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
