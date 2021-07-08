#region Using directives
using System.Collections.Generic;
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
        #region Utilities

        ValueTask AddClass( ElementReference elementRef, string classname );

        ValueTask RemoveClass( ElementReference elementRef, string classname );

        ValueTask ToggleClass( ElementReference elementId, string classname );

        ValueTask AddClassToBody( string classname );

        ValueTask RemoveClassFromBody( string classname );

        ValueTask<bool> ParentHasClass( ElementReference elementRef, string classaname );

        ValueTask SetProperty( ElementReference elementRef, string property, object value );

        ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId );

        ValueTask SetTextValue( ElementReference elementRef, object value );

        ValueTask SetCaret( ElementReference elementRef, int caret );

        ValueTask<int> GetCaret( ElementReference elementRef );

        ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask<string> GetBreakpoint();

        ValueTask ScrollIntoView( string anchorTarget );

        #endregion

        #region Button

        ValueTask InitializeButton( ElementReference elementRef, string elementId, bool preventDefaultSubmit );

        ValueTask DestroyButton( string elementId );

        #endregion

        #region TextEdit

        ValueTask InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask );

        ValueTask DestroyTextEdit( ElementReference elementRef, string elementId );

        #endregion

        #region NumericEdit

        ValueTask InitializeNumericEdit<TValue>( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, object options );

        ValueTask UpdateNumericEdit( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyNumericEdit( ElementReference elementRef, string elementId );

        #endregion

        #region FileEdit

        ValueTask InitializeFileEdit( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId );

        ValueTask DestroyFileEdit( ElementReference elementRef, string elementId );

        ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default );

        ValueTask ResetFileEdit( ElementReference elementRef, string elementId );

        ValueTask OpenFileDialog( ElementReference elementRef, string elementId );

        #endregion

        #region DatePicker

        ValueTask InitializeDatePicker( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyDatePicker( ElementReference elementRef, string elementId );

        ValueTask ActivateDatePicker( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateDatePickerValue( ElementReference elementRef, string elementId, object value );

        ValueTask UpdateDatePickerOptions( ElementReference elementRef, string elementId, object options );

        ValueTask OpenDatePicker( ElementReference elementRef, string elementId );

        ValueTask CloseDatePicker( ElementReference elementRef, string elementId );

        ValueTask ToggleDatePicker( ElementReference elementRef, string elementId );

        #endregion

        #region TimePicker

        ValueTask InitializeTimePicker( ElementReference elementRef, string elementId, object options );

        ValueTask DestroyTimePicker( ElementReference elementRef, string elementId );

        ValueTask ActivateTimePicker( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateTimePickerValue( ElementReference elementRef, string elementId, object value );

        ValueTask UpdateTimePickerOptions( ElementReference elementRef, string elementId, object options );

        ValueTask OpenTimePicker( ElementReference elementRef, string elementId );

        ValueTask CloseTimePicker( ElementReference elementRef, string elementId );

        ValueTask ToggleTimePicker( ElementReference elementRef, string elementId );

        #endregion

        #region Select

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        #endregion

        #region Tooltip

        ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options );

        #endregion

        #region Modal

        ValueTask OpenModal( ElementReference elementRef, bool scrollToTop );

        ValueTask CloseModal( ElementReference elementRef );

        #endregion

        #region Table

        ValueTask InitializeTableFixedHeader( ElementReference elementRef, string elementId );

        ValueTask DestroyTableFixedHeader( ElementReference elementRef, string elementId );

        ValueTask FixedHeaderScrollTableTo( ElementReference elementRef, string elementId, int pixels );

        ValueTask InitializeTableResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode );

        ValueTask DestroyTableResizable( ElementReference elementRef, string elementId );

        #endregion

        #region Closables

        ValueTask RegisterClosableComponent( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef );

        ValueTask UnregisterClosableComponent( ICloseActivator component );

        ValueTask RegisterBreakpointComponent( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId );

        ValueTask UnregisterBreakpointComponent( IBreakpointActivator component );

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
