#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
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

        private static object CreateDotNetObjectRefSyncObj = new object();

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        public DotNetObjectRef<T> CreateDotNetObjectRef<T>( T value ) where T : class
        {
            lock ( CreateDotNetObjectRefSyncObj )
            {
                JSRuntime.SetCurrentJSRuntime( runtime );
                return DotNetObjectRef.Create( value );
            }
        }

        public void DisposeDotNetObjectRef<T>( DotNetObjectRef<T> value ) where T : class
        {
            if ( value != null )
            {
                lock ( CreateDotNetObjectRefSyncObj )
                {
                    JSRuntime.SetCurrentJSRuntime( runtime );
                    value.Dispose();
                }
            }
        }

        public Task<bool> Init( ElementRef elementRef, object componentRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.init", elementRef, DotNetObjectRef.Create( componentRef ) );
        }

        public Task<bool> InitializeTextEdit( string elementId, ElementRef elementRef, string maskType, string editMask )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.initialize", elementId, elementRef, maskType, editMask );
        }

        public Task<bool> DestroyTextEdit( string elementId, ElementRef elementRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.destroy", elementId, elementRef );
        }

        public Task<bool> InitializeNumericEdit( DotNetObjectRef<NumericEditAdapter> dotNetObjectRef, string elementId, ElementRef elementRef, int decimals, string decimalsSeparator, decimal? step )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.initialize", dotNetObjectRef, elementId, elementRef, decimals, decimalsSeparator, step );
        }

        public Task<bool> DestroyNumericEdit( string elementId, ElementRef elementRef )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.destroy", elementId, elementRef );
        }

        public Task<bool> AddClass( ElementRef elementRef, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.addClass", elementRef, classname );
        }

        public Task<bool> RemoveClass( ElementRef elementRef, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.removeClass", elementRef, classname );
        }

        public Task<bool> ToggleClass( ElementRef elementId, string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.toggleClass", elementId, classname );
        }

        public Task<bool> AddClassToBody( string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.addClassToBody", classname );
        }

        public Task<bool> RemoveClassFromBody( string classname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.removeClassFromBody", classname );
        }

        public Task<bool> ParentHasClass( ElementRef elementRef, string classaname )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.parentHasClass", elementRef, classaname );
        }

        /// <summary>
        /// Gets the fake file paths from input field.
        /// </summary>
        /// <param name="element">Input field.</param>
        /// <returns>Returns an array of paths.</returns>
        public Task<string[]> GetFilePaths( ElementRef element )
        {
            return runtime.InvokeAsync<string[]>( $"{BLAZORISE_NAMESPACE}.getFilePaths", element );
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

        public async Task<TValue[]> GetSelectedOptions<TValue>( string elementId )
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
            } ).Where( x => x != default ).ToArray();
        }

        public Task<bool> SetTextValue( ElementRef elementRef, object value )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.setTextValue", elementRef, value );
        }

        public Task RegisterClosableComponent( DotNetObjectRef<CloseActivatorAdapter> dotNetObjectRef, string elementId )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.registerClosableComponent", elementId, dotNetObjectRef );
        }

        public Task UnregisterClosableComponent( ICloseActivator component )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.unregisterClosableComponent", component.ElementId );
        }
    }
}
