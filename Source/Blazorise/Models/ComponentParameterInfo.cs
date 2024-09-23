namespace Blazorise;

/// <summary>
/// Represents a component parameter that can contain the last received value, as well as information about whether it was defined or changed.
/// </summary>
/// <typeparam name="T">The type of the component parameter's value.</typeparam>
public record struct ComponentParameterInfo<T>
{
    #region Members

    private readonly bool defined;

    private readonly bool changed;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentParameterInfo{T}"/> structure with the specified value.
    /// </summary>
    /// <param name="value">The value of the component parameter.</param>
    public ComponentParameterInfo( T value )
    {
        Value = value;
        defined = false;
        changed = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentParameterInfo{T}"/> structure with the specified value and received information.
    /// </summary>
    /// <param name="value">The value of the component parameter.</param>
    /// <param name="defined">A value indicating whether the parameter was received through the component parameters or attributes.</param>
    /// <param name="changed">A value indicating whether the parameter has changed since it was last rendered.</param>
    public ComponentParameterInfo( T value, bool defined, bool changed )
    {
        Value = value;
        this.defined = defined;
        this.changed = changed;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Retrieves the value of the current <see cref="ComponentParameterInfo{T}"/> object, or a default value if the parameter was not defined.
    /// </summary>
    /// <param name="defaultValue">A value to return if the <see cref="Defined"/> property is false.</param>
    /// <returns>The value of the <see cref="Value"/> property if <see cref="Defined"/> is true; otherwise, the <paramref name="defaultValue"/> parameter.</returns>
    public T GetValueOrDefault( T defaultValue )
    {
        if ( !defined )
            return defaultValue;

        return Value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the actual value that was received through the component parameters or attributes.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Indicates whether the parameter was defined and received through the component parameters or attributes.
    /// </summary>
    public bool Defined => defined;

    /// <summary>
    /// Indicates whether the received parameter has changed since the last rendering.
    /// </summary>
    public bool Changed => changed;

    #endregion
}