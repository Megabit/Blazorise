#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public interface IJSRunner
    {
        DotNetObjectRef<T> CreateDotNetObjectRef<T>( T value ) where T : class;

        void DisposeDotNetObjectRef<T>( DotNetObjectRef<T> value ) where T : class;

        Task<bool> Init( ElementReference elementRef, object componentRef );

        Task<bool> InitializeTextEdit( string elementId, ElementReference elementRef, string maskType, string editMask );

        Task<bool> DestroyTextEdit( string elementId, ElementReference elementRef );

        Task<bool> InitializeNumericEdit( DotNetObjectRef<NumericEditAdapter> dotNetObjectRef, string elementId, ElementReference elementRef, int decimals, string decimalsSeparator, decimal? step );

        Task<bool> DestroyNumericEdit( string elementId, ElementReference elementRef );

        Task<bool> InitializeTooltip( string elementId, ElementReference elementRef, ElementReference tooltipRef, ElementReference arrowRef, string placement );

        Task<bool> DestroyTooltip( string elementId );

        Task<bool> AddClass( ElementReference elementRef, string classname );

        Task<bool> RemoveClass( ElementReference elementRef, string classname );

        Task<bool> ToggleClass( ElementReference elementId, string classname );

        Task<bool> AddClassToBody( string classname );

        Task<bool> RemoveClassFromBody( string classname );

        Task<bool> ParentHasClass( ElementReference elementRef, string classaname );

        Task<string[]> GetFilePaths( ElementReference element );

        Task<bool> ActivateDatePicker( string elementId, string formatSubmit );

        Task<TValue[]> GetSelectedOptions<TValue>( string elementId );

        Task<bool> SetTextValue( ElementReference elementRef, object value );

        /// <summary>
        /// Handles the closing of the components that can be toggled.
        /// </summary>
        /// <param name="component">Toggle component.</param>
        /// <returns></returns>
        Task RegisterClosableComponent( DotNetObjectRef<CloseActivatorAdapter> dotNetObjectRef, string elementId );

        Task UnregisterClosableComponent( ICloseActivator component );
    }
}
