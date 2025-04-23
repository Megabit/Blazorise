#region Using directives
using System.Threading.Tasks;
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
    /// Exports data to a specified file with a given MIME type asynchronously.
    /// </summary>
    /// <param name="data">The byte array containing the data to be exported.</param>
    /// <param name="fileName">The name of the file to which the data will be exported.</param>
    /// <param name="mimeType">The MIME type that describes the format of the data being exported.</param>
    /// <returns>An integer indicating the result of the export operation.</returns>
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