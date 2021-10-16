#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Markdown
{
    public class JSMarkdownModule : BaseJSModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSMarkdownModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods        

        public async ValueTask Initialize( DotNetObjectReference<Markdown> dotNetObjectRef, string elementId, string initialValue )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectRef, elementId, initialValue );
        }

        public async ValueTask Destroy( string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", elementId );
        }

        public async ValueTask SetValue( string elementId, string value )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "setValue", elementId, value );
        }

        public async ValueTask<string> GetValue( string elementId )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "getValue", elementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => "./_content/Blazorise.Markdown/blazorise.markdown.js";

        #endregion
    }
}