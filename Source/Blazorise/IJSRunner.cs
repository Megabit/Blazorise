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
        DotNetObjectReference<T> CreateDotNetObjectRef<T>( T value ) where T : class;

        void DisposeDotNetObjectRef<T>( DotNetObjectReference<T> value ) where T : class;

        ValueTask<bool> InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask );

        ValueTask<bool> DestroyTextEdit( ElementReference elementRef, string elementId );

        ValueTask<bool> InitializeNumericEdit( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, int decimals, string decimalsSeparator, decimal? step );

        ValueTask<bool> DestroyNumericEdit( ElementReference elementRef, string elementId );

        ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId );

        ValueTask<bool> InitializeButton( ElementReference elementRef, string elementId, bool preventDefaultSubmit );

        ValueTask<bool> DestroyButton( string elementId );

        ValueTask<bool> AddClass( ElementReference elementRef, string classname );

        ValueTask<bool> RemoveClass( ElementReference elementRef, string classname );

        ValueTask<bool> ToggleClass( ElementReference elementId, string classname );

        ValueTask<bool> AddClassToBody( string classname );

        ValueTask<bool> RemoveClassFromBody( string classname );

        ValueTask<bool> ParentHasClass( ElementReference elementRef, string classaname );

        ValueTask<string[]> GetFilePaths( ElementReference element );

        ValueTask<bool> ActivateDatePicker( string elementId, string formatSubmit );

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask<bool> SetTextValue( ElementReference elementRef, object value );

        ValueTask<bool> OpenModal( ElementReference elementRef, string elementId );

        ValueTask<bool> CloseModal( ElementReference elementRef, string elementId );

        ValueTask<bool> Focus( ElementReference elementRef, string elementId, bool scrollToElement );

        /// <summary>
        /// Handles the closing of the components that can be toggled.
        /// </summary>
        /// <param name="component">Toggle component.</param>
        /// <returns></returns>
        ValueTask<object> RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, string elementId );

        ValueTask<object> UnregisterClosableComponent( ICloseActivator component );
    }
}
