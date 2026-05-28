#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Middleman between the TimePicker component and javascript.
/// </summary>
public class TimePickerAdapter
{
    private readonly ITimePicker timePicker;

    /// <summary>
    /// Default constructor for <see cref="TimePickerAdapter"/>.
    /// </summary>
    /// <param name="timePicker">Time input to which the adapter is referenced.</param>
    public TimePickerAdapter( ITimePicker timePicker )
    {
        this.timePicker = timePicker;
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        return timePicker.OnKeyDownHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyUpHandler( KeyboardEventArgs eventArgs )
    {
        return timePicker.OnKeyUpHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        return timePicker.OnFocusHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        return timePicker.OnFocusInHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnFocusOutHandler( FocusEventArgs eventArgs )
    {
        return timePicker.OnFocusOutHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        return timePicker.OnKeyPressHandler( eventArgs );
    }

    /// <inheritdoc/>
    [JSInvokable]
    public Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        return timePicker.OnBlurHandler( eventArgs );
    }
}