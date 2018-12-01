#region Using directives
using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public class JS
    {
        public static Task<bool> Init( ElementRef elementRef, object componentRef )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.init", elementRef, new DotNetObjectRef( componentRef ) );
        }

        public static Task<bool> AddClass( ElementRef elementRef, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.addClass", elementRef, classname );
        }

        public static Task<bool> RemoveClass( ElementRef elementRef, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.removeClass", elementRef, classname );
        }

        public static Task<bool> ToggleClass( ElementRef elementId, string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.toggleClass", elementId, classname );
        }

        public static Task<bool> AddClassToBody( string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.addClassToBody", classname );
        }

        public static Task<bool> RemoveClassFromBody( string classname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.removeClassFromBody", classname );
        }

        public static Task<bool> ParentHasClass( ElementRef elementRef, string classaname )
        {
            return JSRuntime.Current.InvokeAsync<bool>( "blazorise.parentHasClass", elementRef, classaname );
        }

        /// <summary>
        /// Gets the fake file paths from input field.
        /// </summary>
        /// <param name="element">Input field.</param>
        /// <returns>Returns an array of paths.</returns>
        public static Task<string[]> GetFilePaths( ElementRef element )
        {
            return JSRuntime.Current.InvokeAsync<string[]>( "blazorise.getFilePaths", element );
        }
    }
}
