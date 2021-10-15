#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class JSRunner : IJSRunner
    {
        #region Members

        private readonly IJSRuntime runtime;

        private const string BLAZORISE_NAMESPACE = "blazorise";

        #endregion

        #region Constructors

        public JSRunner( IJSRuntime runtime )
        {
            this.runtime = runtime;
        }

        #endregion

        #region Methods

        #region Utilities

        public ValueTask SetProperty( ElementReference elementRef, string property, object value )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setProperty", elementRef, property, value );
        }

        public ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<DomElement>( $"{BLAZORISE_NAMESPACE}.getElementInfo", elementRef, elementId );
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

        #endregion

        #region TextEdit

        public ValueTask InitializeTextEdit( ElementReference elementRef, string elementId, string maskType, string editMask )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.textEdit.initialize", elementRef, elementId, maskType, editMask );
        }

        public ValueTask DestroyTextEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.textEdit.destroy", elementRef, elementId );
        }

        #endregion

        #region NumericEdit

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

        #endregion

        #region FileEdit

        public ValueTask InitializeFileEdit( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.initialize", dotNetObjectRef, elementRef, elementId );
        }

        public ValueTask DestroyFileEdit( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.fileEdit.destroy", elementRef, elementId );
        }

        public ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default )
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

        #endregion

        #region DatePicker

        public virtual ValueTask InitializeDatePicker( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.initialize", elementRef, elementId, options );
        }

        public virtual ValueTask DestroyDatePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.destroy", elementRef, elementId );
        }

        public virtual ValueTask ActivateDatePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateDatePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.updateValue", elementRef, elementId, value );
        }

        public virtual ValueTask UpdateDatePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.updateOptions", elementRef, elementId, options );
        }

        public virtual ValueTask OpenDatePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.open", elementRef, elementId );
        }

        public virtual ValueTask CloseDatePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.close", elementRef, elementId );
        }

        public virtual ValueTask ToggleDatePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.toggle", elementRef, elementId );
        }

        public virtual ValueTask FocusDatePicker( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.focus", elementRef, elementId, scrollToElement );
        }

        public virtual ValueTask SelectDatePicker( ElementReference elementRef, string elementId, bool focus )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.datePicker.select", elementRef, elementId, focus );
        }

        #endregion

        #region TimePicker

        public virtual ValueTask InitializeTimePicker( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.initialize", elementRef, elementId, options );
        }

        public virtual ValueTask DestroyTimePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.destroy", elementRef, elementId );
        }

        public virtual ValueTask ActivateTimePicker( ElementReference elementRef, string elementId, object options )
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask UpdateTimePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.updateOptions", elementRef, elementId, options );
        }

        public virtual ValueTask UpdateTimePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.updateValue", elementRef, elementId, value );
        }

        public virtual ValueTask OpenTimePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.open", elementRef, elementId );
        }

        public virtual ValueTask CloseTimePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.close", elementRef, elementId );
        }

        public virtual ValueTask ToggleTimePicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.toggle", elementRef, elementId );
        }

        public virtual ValueTask FocusTimePicker( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.focus", elementRef, elementId, scrollToElement );
        }

        public virtual ValueTask SelectTimePicker( ElementReference elementRef, string elementId, bool focus )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.timePicker.select", elementRef, elementId, focus );
        }

        #endregion

        #region ColorPicker

        public virtual ValueTask InitializeColorPicker( DotNetObjectReference<ColorPicker> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.initialize", dotNetObjectRef, elementRef, elementId, options );
        }

        public virtual ValueTask DestroyColorPicker( ElementReference elementRef, string elementId )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.destroy", elementRef, elementId );
        }

        public virtual ValueTask UpdateColorPickerValue( ElementReference elementRef, string elementId, object value )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.updateValue", elementRef, elementId, value );
        }

        public virtual ValueTask UpdateColorPickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.updateOptions", elementRef, elementId, options );
        }

        public virtual ValueTask UpdateColorPickerLocalization( ElementReference elementRef, string elementId, object localization )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.updateLocalization", elementRef, elementId, localization );
        }

        public virtual ValueTask FocusColorPicker( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.focus", elementRef, elementId, scrollToElement );
        }

        public virtual ValueTask SelectColorPicker( ElementReference elementRef, string elementId, bool focus )
        {
            return Runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.colorPicker.select", elementRef, elementId, focus );
        }

        #endregion

        #region Select

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
            } ).ToArray();
        }

        public ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.setSelectedOptions", elementId, values );
        }

        public ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.focus", elementRef, elementId, scrollToElement );
        }

        public ValueTask Select( ElementReference elementRef, string elementId, bool focus )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.select", elementRef, elementId, focus );
        }

        public ValueTask ScrollIntoView( string anchorTarget )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.link.scrollIntoView", anchorTarget );
        }

        #endregion

        #region Theme

        public virtual ValueTask AddThemeVariable( string name, string value )
        {
            return runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.theme.addVariable", name, value );
        }

        #endregion

        #region Table

        public ValueTask InitializeTableFixedHeader( ElementReference elementRef, string elementId )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.initializeTableFixedHeader", elementRef, elementId );

        public ValueTask DestroyTableFixedHeader( ElementReference elementRef, string elementId )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.destroyTableFixedHeader", elementRef, elementId );

        public ValueTask FixedHeaderScrollTableToPixels( ElementReference elementRef, string elementId, int pixels )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.fixedHeaderScrollTableToPixels", elementRef, elementId, pixels );

        public ValueTask FixedHeaderScrollTableToRow( ElementReference elementRef, string elementId, int row )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.fixedHeaderScrollTableToRow", elementRef, elementId, row );

        public ValueTask InitializeTableResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.initializeResizable", elementRef, elementId, resizeMode );

        public ValueTask DestroyTableResizable( ElementReference elementRef, string elementId )
            => runtime.InvokeVoidAsync( $"{BLAZORISE_NAMESPACE}.table.destroyResizable", elementRef, elementId );

        #endregion

        #endregion

        #region Properties

        protected IJSRuntime Runtime => runtime;

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
