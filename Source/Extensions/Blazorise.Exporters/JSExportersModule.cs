#region Using directives
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Exporters;

public class JSExportersModule : BaseJSModule
{
    #region Constructors

    public JSExportersModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
    : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Exports the given data to a file.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fileName"></param>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    public virtual async ValueTask<int> ExportToFile( byte[] data, string fileName, string mimeType )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<int>( "exportToFile", data, fileName, mimeType );
    }

    /// <summary>
    /// Copies the specified string content to the clipboard.
    /// </summary>
    /// <param name="stringToCopy">The string content to copy to the clipboard.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask CopyStringToClipboard( string stringToCopy )
        => InvokeSafeVoidAsync( "copyStringToClipboard", stringToCopy );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Exporters/exporters.js?v={VersionProvider.Version}";

    #endregion
}
