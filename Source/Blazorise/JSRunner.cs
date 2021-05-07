﻿#region Using directives
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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class JSRunner : IJSRunner
    {
        #region Members

        private readonly IJSRuntime runtime;

        private const string BLAZORISE_NAMESPACE = "blazorise";

        #endregion

        #region Methods

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        public ValueTask InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.textEdit.initialize", elementRef, elementId, maskType, editMask );
        }

        public ValueTask DestroyTextEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.textEdit.destroy", elementRef, elementId );
        }

        public ValueTask InitializeNumericEdit<TValue>( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.numericEdit.initialize", dotNetObjectRef, elementRef, elementId, options );
        }

        public ValueTask UpdateNumericEdit( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.numericEdit.update", elementRef, elementId, options );
        }

        public ValueTask DestroyNumericEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.numericEdit.destroy", elementRef, elementId );
        }

        public virtual ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.tooltip.initialize", elementRef, elementId, options );
        }

        public virtual ValueTask InitializeButton( ElementReference elementRef, string elementId, bool preventDefaultSubmit )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.button.initialize", elementRef, elementId, preventDefaultSubmit );
        }

        public ValueTask DestroyButton( string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.button.destroy", elementId );
        }

        public ValueTask AddClass( ElementReference elementRef, string classname )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.addClass", elementRef, classname );
        }

        public ValueTask RemoveClass( ElementReference elementRef, string classname )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.removeClass", elementRef, classname );
        }

        public ValueTask ToggleClass( ElementReference elementId, string classname )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.toggleClass", elementId, classname );
        }

        public ValueTask AddClassToBody( string classname )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.addClassToBody", classname );
        }

        public ValueTask RemoveClassFromBody( string classname )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.removeClassFromBody", classname );
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

        public virtual ValueTask InitializeDatePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask DestroyDatePicker( ElementReference elementRef, string elementId )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask ActivateDatePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateDatePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateDatePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask InitializeTimePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask DestroyTimePicker( ElementReference elementRef, string elementId )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask ActivateTimePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateTimePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateTimePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return ValueTask.CompletedTask;
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

        public ValueTask SetTextValue( ElementReference elementRef, object value )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setTextValue", elementRef, value );
        }

        public ValueTask SetCaret( ElementReference elementRef, int caret )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setCaret", elementRef, caret );
        }

        public ValueTask<int> GetCaret( ElementReference elementRef )
        {
            return runtime.InvokeAsync<int>( $"{BLAZORISE_NAMESPACE}.getCaret", elementRef );
        }

        public abstract ValueTask OpenModal( ElementReference elementRef, bool scrollToTop );

        public abstract ValueTask CloseModal( ElementReference elementRef );

        public ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.focus", elementRef, elementId, scrollToElement );
        }

        public ValueTask RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.registerClosableComponent", elementRef, dotNetObjectRef );
        }

        public ValueTask UnregisterClosableComponent( ICloseActivator component )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.unregisterClosableComponent", component.ElementRef );
        }

        public ValueTask RegisterBreakpointComponent( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.breakpoint.registerBreakpointComponent", elementId, dotNetObjectRef );
        }

        public ValueTask UnregisterBreakpointComponent( IBreakpointActivator component )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.breakpoint.unregisterBreakpointComponent", component.ElementId );
        }

        public ValueTask<string> GetBreakpoint()
        {
            return runtime.InvokeAsync<string>( $"{BLAZORISE_NAMESPACE}.breakpoint.getBreakpoint" );
        }

        public ValueTask ScrollIntoView( string anchorTarget )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.link.scrollIntoView", anchorTarget );
        }

        public ValueTask InitializeFileEdit( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.initialize", dotNetObjectRef, elementRef, elementId );
        }

        public ValueTask DestroyFileEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.destroy", elementRef, elementId );
        }

        public ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken )
        {
            return runtime.InvokeAsync<string>( $"{BLAZORISE_NAMESPACE}.fileEdit.readFileData", elementRef, fileEntryId, position, length );
        }

        public ValueTask ResetFileEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.reset", elementRef, elementId );
        }

        public ValueTask OpenFileDialog( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.open", elementRef, elementId );
        }

        public ValueTask InitializeTableFixedHeader( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.initializeTableFixedHeader", elementRef, elementId );
        }

        #endregion

        #region Properties

        protected IJSRuntime Runtime => runtime;

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
