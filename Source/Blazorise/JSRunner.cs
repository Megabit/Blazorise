#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public abstract class JSRunner : IJSRunner
    {
        protected readonly IJSRuntime runtime;

        private const string BLAZORISE_NAMESPACE = "blazorise";

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        public ValueTask<bool> InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.initialize", elementRef, elementId, maskType, editMask );
        }

        public ValueTask<bool> DestroyTextEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.textEdit.destroy", elementRef, elementId );
        }

        public ValueTask<bool> InitializeNumericEdit<TValue>( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, int decimals, string decimalsSeparator, decimal? step, TValue min, TValue max )
        {
            // find the min and max possible value based on the supplied value type
            var (minFromType, maxFromType) = Converters.GetMinMaxValueOfType<TValue>();

            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.initialize",
                dotNetObjectRef,
                elementRef,
                elementId,
                decimals,
                decimalsSeparator,
                step,
                min.IsEqual( default ) ? minFromType : min,
                max.IsEqual( default ) ? maxFromType : max );
        }

        public ValueTask<bool> DestroyNumericEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.numericEdit.destroy", elementRef, elementId );
        }

        public virtual ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.tooltip.initialize", elementRef, elementId );
        }

        public virtual ValueTask<bool> InitializeButton( ElementReference elementRef, string elementId, bool preventDefaultSubmit )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.button.initialize", elementRef, elementId, preventDefaultSubmit );
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

        public async ValueTask SetProperty( ElementReference elementRef, string property, object value )
        {
            await runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setProperty", elementRef, property, value );
        }

        public ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<DomElement>( $"{BLAZORISE_NAMESPACE}.getElementInfo", elementRef, elementId );
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

        /// <summary>
        /// Activates the time picker for a given element id.
        /// </summary>
        /// <param name="elementId">Input element id.</param>
        /// <param name="formatSubmit">Date format to submit.</param>
        public virtual ValueTask<bool> ActivateTimePicker( string elementId, string formatSubmit )
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

        public ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setSelectedOptions", elementId, values );
        }

        public ValueTask<bool> SetTextValue( ElementReference elementRef, object value )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.setTextValue", elementRef, value );
        }

        public ValueTask SetCaret( ElementReference elementRef, int caret )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setCaret", elementRef, caret );
        }

        public ValueTask<int> GetCaret( ElementReference elementRef )
        {
            return runtime.InvokeAsync<int>( $"{BLAZORISE_NAMESPACE}.getCaret", elementRef );
        }

        public abstract ValueTask<bool> OpenModal( ElementReference elementRef, bool scrollToTop );

        public abstract ValueTask<bool> CloseModal( ElementReference elementRef );

        public ValueTask<bool> OpenFileDialog( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.fileEdit.open", elementRef, elementId );
        }

        public ValueTask<bool> Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.focus", elementRef, elementId, scrollToElement );
        }

        public ValueTask<object> RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.registerClosableComponent", elementRef, dotNetObjectRef );
        }

        public ValueTask<object> UnregisterClosableComponent( ICloseActivator component )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.unregisterClosableComponent", component.ElementRef );
        }

        public ValueTask<object> RegisterBreakpointComponent( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.breakpoint.registerBreakpointComponent", elementId, dotNetObjectRef );
        }

        public ValueTask<object> UnregisterBreakpointComponent( IBreakpointActivator component )
        {
            return runtime.InvokeAsync<object>( $"{BLAZORISE_NAMESPACE}.breakpoint.unregisterBreakpointComponent", component.ElementId );
        }

        public ValueTask<string> GetBreakpoint()
        {
            return runtime.InvokeAsync<string>( $"{BLAZORISE_NAMESPACE}.breakpoint.getBreakpoint" );
        }

        public ValueTask<bool> ScrollIntoView( string anchorTarget )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.link.scrollIntoView", anchorTarget );
        }

        public ValueTask<bool> InitializeFileEdit( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.fileEdit.initialize", dotNetObjectRef, elementRef, elementId );
        }

        public ValueTask<bool> DestroyFileEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BLAZORISE_NAMESPACE}.fileEdit.destroy", elementRef, elementId );
        }

        public ValueTask<string> ReadDataAsync( CancellationToken cancellationToken, ElementReference elementRef, int fileEntryId, long position, long length )
        {
            return runtime.InvokeAsync<string>( $"{BLAZORISE_NAMESPACE}.fileEdit.readFileData", cancellationToken, elementRef, fileEntryId, position, length );
        }

        public ValueTask ResetFileEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.reset", elementRef, elementId );
        }
    }
}
