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

        #region Select

        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );

        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        #endregion
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
