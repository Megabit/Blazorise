#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IJSRunner
    {
        ValueTask InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask );

        ValueTask DestroyTextEdit( ElementReference elementRef, string elementId );

        ValueTask InitializeNumericEdit<TValue>( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, object options );

        ValueTask UpdateNumericEdit( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyNumericEdit( ElementReference elementRef, string elementId );

        ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options );

        ValueTask InitializeButton( ElementReference elementRef, string elementId, bool preventDefaultSubmit );

        ValueTask DestroyButton( string elementId );

        ValueTask AddClass( ElementReference elementRef, string classname );

        ValueTask RemoveClass( ElementReference elementRef, string classname );

        ValueTask ToggleClass( ElementReference elementId, string classname );

        ValueTask AddClassToBody( string classname );

        ValueTask RemoveClassFromBody( string classname );

        ValueTask<bool> ParentHasClass( ElementReference elementRef, string classaname );

        ValueTask SetProperty( ElementReference elementRef, string property, object value );

        ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId );

        ValueTask InitializeDatePicker( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyDatePicker( ElementReference elementRef, string elementId );

        ValueTask ActivateDatePicker( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateDatePickerValue( ElementReference elementRef, string elementId, object value );

        ValueTask UpdateDatePickerOptions( ElementReference elementRef, string elementId, object options );

        ValueTask InitializeTimePicker( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyTimePicker( ElementReference elementRef, string elementId );

        ValueTask ActivateTimePicker( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateTimePickerValue( ElementReference elementRef, string elementId, object value );

        ValueTask UpdateTimePickerOptions( ElementReference elementRef, string elementId, object options );

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        ValueTask SetTextValue( ElementReference elementRef, object value );

        ValueTask SetCaret( ElementReference elementRef, int caret );

        ValueTask<int> GetCaret( ElementReference elementRef );

        ValueTask OpenModal( ElementReference elementRef, bool scrollToTop );

        ValueTask CloseModal( ElementReference elementRef );

        ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef );

        ValueTask UnregisterClosableComponent( ICloseActivator component );

        ValueTask RegisterBreakpointComponent( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId );

        ValueTask UnregisterBreakpointComponent( IBreakpointActivator component );

        ValueTask<string> GetBreakpoint();

        ValueTask ScrollIntoView( string anchorTarget );

        ValueTask InitializeFileEdit( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId );

        ValueTask DestroyFileEdit( ElementReference elementRef, string elementId );

        ValueTask<string> ReadDataAsync( CancellationToken cancellationToken, ElementReference elementRef, int fileEntryId, long position, long length );

        ValueTask ResetFileEdit( ElementReference elementRef, string elementId );

        ValueTask OpenFileDialog( ElementReference elementRef, string elementId );
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
