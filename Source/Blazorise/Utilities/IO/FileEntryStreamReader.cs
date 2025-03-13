#region Using directives
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public abstract class FileEntryStreamReader
{
    private readonly IJSFileModule jsModule;
    private readonly ElementReference elementRef;
    private readonly IFileEntry fileEntry;
    private readonly IFileEntryNotifier fileEntryNotifier;

    public FileEntryStreamReader( IJSFileModule jsModule, ElementReference elementRef, IFileEntry fileEntry, IFileEntryNotifier fileEntryNotifier )
    {
        this.jsModule = jsModule;
        this.elementRef = elementRef;
        this.fileEntry = fileEntry;
        this.fileEntryNotifier = fileEntryNotifier;
    }

    protected IJSFileModule JSModule => jsModule;

    protected ElementReference ElementRef => elementRef;

    protected IFileEntry FileEntry => fileEntry;

    protected IFileEntryNotifier FileEntryNotifier => fileEntryNotifier;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member