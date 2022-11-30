namespace Blazorise;

/// <summary>
/// Represents a component parameter that can containe the last received value.
/// </summary>
/// <typeparam name="T"></typeparam>
public record struct ComponentParameterInfo<T>
{
    private readonly bool received;

    private readonly bool changed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentParameterInfo{T}"/> structure to the specified value.
    /// </summary>
    /// <param name="value">A value type.</param>
    public ComponentParameterInfo( T value )
    {
        Value = value;

        received = false;
        changed = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentParameterInfo{T}"/> structure to the specified value and received information.
    /// </summary>
    /// <param name="value">A value type.</param>
    /// <param name="received">True if the parameter has being received through the parameters.</param>
    /// <param name="changed">True if the parameter has being changes since the last time.</param>
    public ComponentParameterInfo( T value, bool received, bool changed )
    {
        Value = value;

        this.received = received;
        this.changed = changed;
    }

    /// <summary>
    /// Gets the actual value that was received through the supplied parameters.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Indicates if the parameter was being defined and received through the component parameters or attributes.
    /// </summary>
    public bool Received => received;

    /// <summary>
    /// Indicates if the received parameter has being changed since the last rendering. 
    /// </summary>
    public bool Changed => changed;

    /// <summary>
    /// Retrieves the value of the current <see cref="ComponentParameterInfo{T}"/> object, or a default value.
    /// </summary>
    /// <param name="defaultValue">A value to return if the <see cref="Received"/> property is false.</param>
    /// <returns>The value of the <see cref="Value"/> property if the <see cref="Received"/> property is true; otherwise, the <paramref name="defaultValue"/> parameter.</returns>
    /// <remarks>
    /// The <see cref="GetValueOrDefault"/> method returns a value even if the <see cref="Received"/> property is false.
    /// </remarks>
    public T GetValueOrDefault( T defaultValue )
    {
        if ( !received )
            return defaultValue;

        return Value;
    }
}