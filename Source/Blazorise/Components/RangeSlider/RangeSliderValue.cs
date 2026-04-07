namespace Blazorise;

/// <summary>
/// Represents the selected value range for a RangeSlider component.
/// </summary>
/// <typeparam name="TValue">Data type used by the slider handles.</typeparam>
public readonly record struct RangeSliderValue<TValue>( TValue Start, TValue End )
{
    /// <summary>
    /// Implicitly converts a tuple to a <see cref="RangeSliderValue{TValue}"/>.
    /// </summary>
    /// <param name="value">Tuple that contains the range start and end values.</param>
    public static implicit operator RangeSliderValue<TValue>( (TValue Start, TValue End) value )
        => new( value.Start, value.End );

    /// <summary>
    /// Implicitly converts a <see cref="RangeSliderValue{TValue}"/> to a tuple.
    /// </summary>
    /// <param name="value">Range value to convert.</param>
    public static implicit operator (TValue Start, TValue End)( RangeSliderValue<TValue> value )
        => (value.Start, value.End);
}