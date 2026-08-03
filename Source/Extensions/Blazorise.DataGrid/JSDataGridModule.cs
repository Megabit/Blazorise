#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Supports js data grid module behavior in DataGrid components.
/// </summary>
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

    /// <summary>
    /// Initializes browser integration for the js data grid module.
    /// </summary>
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

    /// <summary>
    /// Enables keyboard navigation between table cells.
    /// </summary>
    public virtual async ValueTask InitializeTableCellNavigation( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initializeTableCellNavigation", elementRef, elementId );
    }

    /// <summary>
    /// Removes focus from the active cell editor.
    /// </summary>
    public virtual async ValueTask BlurActiveCellEditor( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "blurActiveCellEditor", elementRef, elementId );
    }

    /// <summary>
    /// Returns cell width.
    /// </summary>
    public virtual async ValueTask<int> GetCellWidth( ElementReference elementRef, string elementId, int rowIndex, string columnId )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<int>( "getCellWidth", elementRef, elementId, rowIndex, columnId );
    }

    /// <summary>
    /// Scrolls the grid container to the requested position.
    /// </summary>
    public virtual async ValueTask<int> ScrollTo( ElementReference elementRef, string classname )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<int>( "scrollTo", elementRef, classname );
    }

    /// <summary>
    /// Brings a virtualized row into the visible grid viewport.
    /// </summary>
    public virtual async ValueTask ScrollVirtualizedRowIntoView( ElementReference elementRef, string elementId, int rowIndex )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "scrollVirtualizedRowIntoView", elementRef, elementId, rowIndex );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.DataGrid/datagrid.js?v={VersionProvider.Version}";

    #endregion
}