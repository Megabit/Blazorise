#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation for the utilities JS module.
    /// </summary>
    public class JSUtilitiesModule : BaseJSModule, IJSUtilitiesModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSUtilitiesModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask AddClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "addClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "removeClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask ToggleClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "toggleClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask AddClassToBody( string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "addClassToBody", classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveClassFromBody( string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "removeClassFromBody", classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<bool> ParentHasClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "parentHasClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "focus", elementRef, elementId, scrollToElement );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Select( ElementReference elementRef, string elementId, bool focus )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "select", elementRef, elementId, focus );
        }

        /// <inheritdoc/>
        public virtual async ValueTask ScrollAnchorIntoView( string anchorTarget )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "scrollAnchorIntoView", anchorTarget );
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetCaret( ElementReference elementRef, int caret )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "setCaret", elementRef, caret );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<int> GetCaret( ElementReference elementRef )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<int>( "getCaret", elementRef );
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetTextValue( ElementReference elementRef, object value )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeAsync<int>( "setTextValue", elementRef, value );
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetProperty( ElementReference elementRef, string property, object value )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "setProperty", elementRef, property, value );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<DomElement>( "getElementInfo", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string> GetUserAgent()
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "getUserAgent" );
        }

        /// <inheritdoc/>
        public async ValueTask CopyToClipboard( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "copyToClipboard", elementRef, elementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/utilities.js?v={VersionProvider.Version}";

        #endregion
    }
}