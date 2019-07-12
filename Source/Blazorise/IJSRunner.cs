#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public interface IJSRunner
    {
        Task<bool> Init( ElementRef elementRef, object componentRef );

        Task<bool> Initialize( string path, string elementId, ElementRef elementRef, string mask );

        Task<bool> Destroy( string path, string elementId, ElementRef elementRef, string mask );

        Task<bool> AddClass( ElementRef elementRef, string classname );

        Task<bool> RemoveClass( ElementRef elementRef, string classname );

        Task<bool> ToggleClass( ElementRef elementId, string classname );

        Task<bool> AddClassToBody( string classname );

        Task<bool> RemoveClassFromBody( string classname );

        Task<bool> ParentHasClass( ElementRef elementRef, string classaname );

        Task<string[]> GetFilePaths( ElementRef element );

        Task<bool> ActivateDatePicker( string elementId, string formatSubmit );

        Task<TValue[]> GetSelectedOptions<TValue>( string elementId );

        Task<bool> SetTextValue( ElementRef elementRef, object value );

        /// <summary>
        /// Handles the closing of the components that can be toggled.
        /// </summary>
        /// <param name="component">Toggle component.</param>
        /// <returns></returns>
        Task RegisterClosableComponent( ICloseActivator component );

        Task UnregisterClosableComponent( ICloseActivator component );
    }
}
