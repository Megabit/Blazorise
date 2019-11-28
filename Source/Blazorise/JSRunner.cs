#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public abstract class JSRunner : IJSRunner
    {
        protected readonly IJSRuntime runtime;

        protected const string BLAZORISE_NAMESPACE = "blazorise";

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        public DotNetObjectReference<T> CreateDotNetObjectRef<T>( T value ) where T : class
        {
            return DotNetObjectReference.Create( value );
        }

        public void DisposeDotNetObjectRef<T>( DotNetObjectReference<T> value ) where T : class
        {
            if ( value != null )
            {
                value.Dispose();
            }
        }

        public ValueTask<bool> Init( ElementReference elementRef, object componentRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.init", elementRef, DotNetObjectReference.Create( componentRef ) );
        }

        public ValueTask<bool> InitializeTextEdit( string elementId, ElementReference elementRef, string maskType, string editMask )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.initialize", elementId, elementRef, maskType, editMask );
        }

        public ValueTask<bool> DestroyTextEdit( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.destroy", elementId, elementRef );
        }

        public ValueTask<bool> InitializeNumericEdit( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, string elementId, ElementReference elementRef, int decimals, string decimalsSeparator, decimal? step )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.initialize", dotNetObjectRef, elementId, elementRef, decimals, decimalsSeparator, step );
        }

        public ValueTask<bool> DestroyNumericEdit( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.destroy", elementId, elementRef );
        }

        public virtual ValueTask<bool> InitializeTooltip( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.tooltip.initialize", elementId, elementRef );
        }

        public virtual ValueTask<bool> InitializeButton( string elementId, ElementReference elementRef, bool preventDefaultSubmit )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.button.initialize", elementId, elementRef, preventDefaultSubmit );
        }

        public ValueTask<bool> DestroyButton( string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.button.destroy", elementId );
        }

        public ValueTask<bool> AddClass( ElementReference elementRef, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.addClass", elementRef, classname );
        }

        public ValueTask<bool> RemoveClass( ElementReference elementRef, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.removeClass", elementRef, classname );
        }

        public ValueTask<bool> ToggleClass( ElementReference elementId, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.toggleClass", elementId, classname );
        }

        public ValueTask<bool> AddClassToBody( string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.addClassToBody", classname );
        }

        public ValueTask<bool> RemoveClassFromBody( string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.removeClassFromBody", classname );
        }

        public ValueTask<bool> ParentHasClass( ElementReference elementRef, string classaname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.parentHasClass", elementRef, classaname );
        }

        /// <summary>
        /// Gets the fake file paths from input field.
        /// </summary>
        /// <param name="element">Input field.</param>
        /// <returns>Returns an array of paths.</returns>
        public ValueTask<string[]> GetFilePaths( ElementReference element )
        {
            return runtime.InvokeAsync<string[]>( $"{BLAZORISE_NAMESPACE}.getFilePaths", element );
        }

        /// <summary>
        /// Activates the date picker for a given element id.
        /// </summary>
        /// <param name="elementId">Input element id.</param>
        /// <param name="formatSubmit">Date format to submit.</param>
        public virtual ValueTask<bool> ActivateDatePicker( string elementId, string formatSubmit )
        {
            // must be implemented by a framework provider!
            return new ValueTask<bool>( true );
        }

        public async ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId )
        {
            // All of this is because Blazor is not serializing types as it should! In this case nullable types
            // are not working (enum?, int?, etc.) so we need to do it manually.

            // get the selected values for JS as strings
            var stringValues = await runtime.InvokeAsync<string[]>( $"{BLAZORISE_NAMESPACE}.getSelectedOptions", elementId );

            return stringValues?.Select( value =>
            {
                try
                {
                    if ( string.IsNullOrEmpty( value ) )
                        return default;

                    return Converters.ChangeType<TValue>( value );
                }
                catch
                {
                    return default;
                }
            } ).Distinct().ToArray();
        }

        public ValueTask<bool> SetTextValue( ElementReference elementRef, object value )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.setTextValue", elementRef, value );
        }

        public ValueTask<object> RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, string elementId )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.registerClosableComponent", elementId, dotNetObjectRef );
        }

        public ValueTask<object> UnregisterClosableComponent( ICloseActivator component )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.unregisterClosableComponent", component.ElementId );
        }
    }
}
