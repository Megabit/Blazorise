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

        ValueTask SetProperty( ElementReference elementRef, string property, object value );

        ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId );

        ValueTask SetTextValue( ElementReference elementRef, object value );

        ValueTask SetCaret( ElementReference elementRef, int caret );

        ValueTask<int> GetCaret( ElementReference elementRef );

        ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask Select( ElementReference elementRef, string elementId, bool focus );

        ValueTask ScrollIntoView( string anchorTarget );

        #endregion

        #region Theme

        ValueTask AddThemeVariable( string name, string value );

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

        ValueTask FocusDatePicker( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask SelectDatePicker( ElementReference elementRef, string elementId, bool focus );

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

        ValueTask FocusTimePicker( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask SelectTimePicker( ElementReference elementRef, string elementId, bool focus );

        #endregion

        #region ColorPicker

        ValueTask InitializeColorPicker( DotNetObjectReference<ColorPicker> dotNetObjectRef, ElementReference elementRef, string elementId, object options );

        ValueTask DestroyColorPicker( ElementReference elementRef, string elementId );

        ValueTask UpdateColorPickerValue( ElementReference elementRef, string elementId, object value );

        ValueTask UpdateColorPickerOptions( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateColorPickerLocalization( ElementReference elementRef, string elementId, object localization );

        ValueTask FocusColorPicker( ElementReference elementRef, string elementId, bool scrollToElement );

        ValueTask SelectColorPicker( ElementReference elementRef, string elementId, bool focus );

        #endregion

        #region Select

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        #endregion

        #region Table

        ValueTask InitializeTableFixedHeader( ElementReference elementRef, string elementId );

        ValueTask DestroyTableFixedHeader( ElementReference elementRef, string elementId );

        ValueTask FixedHeaderScrollTableToPixels( ElementReference elementRef, string elementId, int pixels );

        ValueTask FixedHeaderScrollTableToRow( ElementReference elementRef, string elementId, int row );

        ValueTask InitializeTableResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode );

        ValueTask DestroyTableResizable( ElementReference elementRef, string elementId );

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
