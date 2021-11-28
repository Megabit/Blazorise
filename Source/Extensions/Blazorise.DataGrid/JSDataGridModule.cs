﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid
{
    public class JSDataGridModule : BaseJSModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSDataGridModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods        

        public virtual async ValueTask Initialize( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", elementRef, elementId );
        }


        public virtual async ValueTask<int> ScrollTo( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<int>( "scrollTo", elementRef, classname );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.DataGrid/datagrid.js?v={VersionProvider.Version}";

        #endregion
    }
}
