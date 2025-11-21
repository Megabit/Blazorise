#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Middleman between the NumericPicker component and javascript.
/// </summary>
public class NumericPickerAdapter
{
    private readonly INumericPicker numericPicker;

    /// <summary>
    /// Default constructor for <see cref="NumericPickerAdapter"/>.
    /// </summary>
    /// <param name="numericPicker">Numeric input to which the adapter is referenced.</param>
    public NumericPickerAdapter( INumericPicker numericPicker )
    {
        this.numericPicker = numericPicker;
    }

    /// <summary>
    /// Notify us from JS that input value changed.
    /// </summary>
    /// <param name="value">New input value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task SetValue( string value )
    {
        return numericPicker.SetValue( value );
    }
}