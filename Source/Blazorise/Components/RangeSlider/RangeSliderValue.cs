namespace Blazorise;

/// <summary>
/// Represents the selected value range for a <see cref="RangeSlider{TValue}"/>.
/// </summary>
/// <typeparam name="TValue">Data type used by the slider handles.</typeparam>
public readonly record struct RangeSliderValue<TValue>( TValue Start, TValue End );
