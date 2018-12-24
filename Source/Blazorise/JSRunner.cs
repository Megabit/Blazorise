#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public abstract class JSRunner : IJSRunner
    {
        public Task<bool> Init( ElementRef elementRef, object componentRef )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.init", elementRef, new DotNetObjectRef( componentRef ) );
        }

        public Task<bool> AddClass( ElementRef elementRef, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.addClass", elementRef, classname );
        }

        public Task<bool> RemoveClass( ElementRef elementRef, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.removeClass", elementRef, classname );
        }

        public Task<bool> ToggleClass( ElementRef elementId, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.toggleClass", elementId, classname );
        }

        public Task<bool> AddClassToBody( string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.addClassToBody", classname );
        }

        public Task<bool> RemoveClassFromBody( string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.removeClassFromBody", classname );
        }

        public Task<bool> ParentHasClass( ElementRef elementRef, string classaname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.parentHasClass", elementRef, classaname );
        }

        /// <summary>
        /// Gets the fake file paths from input field.
        /// </summary>
        /// <param name="element">Input field.</param>
        /// <returns>Returns an array of paths.</returns>
        public Task<string[]> GetFilePaths( ElementRef element )
        {
            return JSRuntime.Current.InvokeAsync<string[]>( "blazorise.getFilePaths", element );
        }

        /// <summary>
        /// Activates the date picker for a given element id.
        /// </summary>
        /// <param name="elementId">Input element id.</param>
        /// <param name="formatSubmit">Date format to submit.</param>
        public virtual Task<bool> ActivateDatePicker( string elementId, string formatSubmit )
        {
            // must be implemented by a framework provider!
            return Task.FromResult( true );
        }
    }
}
