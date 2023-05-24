#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Middleman between the DatePicker component and javascript.
/// </summary>
public class DatePickerAdapter
{
    private readonly IDatePicker datePicker;

    /// <summary>
    /// Default constructor for <see cref="DatePickerAdapter"/>.
    /// </summary>
    /// <param name="datePicker">Date input to which the adapter is referenced.</param>
    public DatePickerAdapter( IDatePicker datePicker )
    {
        this.datePicker = datePicker;
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        return datePicker.OnKeyDownHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyUpHandler( KeyboardEventArgs eventArgs )
    {
        return datePicker.OnKeyUpHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        return datePicker.OnFocusHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        return datePicker.OnFocusInHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusOutHandler( FocusEventArgs eventArgs )
    {
        return datePicker.OnFocusOutHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        return datePicker.OnKeyPressHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        return datePicker.OnBlurHandler( eventArgs );
    }
}