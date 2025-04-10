#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid;

public class JSDataGridModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSDataGridModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods        

    public virtual async ValueTask Initialize( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    public virtual async ValueTask InitializeTableCellNavigation( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initializeTableCellNavigation", elementRef, elementId );
    }


    public virtual async ValueTask<int> ScrollTo( ElementReference elementRef, string classname )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<int>( "scrollTo", elementRef, classname );
    }

    
    public virtual async ValueTask<int> ExportToFile( byte[] data, string fileName, string mimeType )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<int>( "exportToFile", data, fileName, mimeType );
    }
    
    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.DataGrid/datagrid.js?v={VersionProvider.Version}";

    #endregion
}