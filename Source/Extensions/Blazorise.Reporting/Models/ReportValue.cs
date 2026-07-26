#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Stores a report property value together with an optional formula that can override it at render time.
/// </summary>
/// <typeparam name="T">The property value type.</typeparam>
public sealed class ReportValue<T>
{
    #region Methods

    /// <summary>
    /// Creates a report value from a static property value.
    /// </summary>
    /// <param name="value">Static property value.</param>
    public static implicit operator ReportValue<T>( T value )
    {
        return new() { Value = value };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Static fallback value used when no formula is defined or formula evaluation fails.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Formula expression evaluated at render time.
    /// </summary>
    public string Formula { get; set; }

    /// <summary>
    /// Indicates that the value has a formula expression.
    /// </summary>
    [JsonIgnore]
    public bool HasFormula => !string.IsNullOrWhiteSpace( Formula );

    #endregion
}

/// <summary>
/// Factory helpers for report values.
/// </summary>
public static class ReportValue
{
    #region Methods

    /// <summary>
    /// Creates a report value with a static fallback value and optional formula.
    /// </summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="value">Static fallback value.</param>
    /// <param name="formula">Formula expression evaluated at render time.</param>
    /// <returns>A report value instance.</returns>
    public static ReportValue<T> Create<T>( T value, string formula = null )
    {
        return new()
        {
            Value = value,
            Formula = formula,
        };
    }

    /// <summary>
    /// Creates a report value that is primarily driven by a formula.
    /// </summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="formula">Formula expression evaluated at render time.</param>
    /// <param name="fallback">Static fallback value used when formula evaluation fails.</param>
    /// <returns>A report value instance.</returns>
    public static ReportValue<T> Formula<T>( string formula, T fallback = default )
    {
        return new()
        {
            Value = fallback,
            Formula = formula,
        };
    }

    #endregion
}