#region Using directives
using System;
using System.Globalization;
using Blazorise.PivotGrid.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Holds information about a PivotGrid result field without requiring a Blazor component instance.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridFieldInfo<TItem>
{
    #region Members

    private Func<TItem, object> valueGetter;
    private string valueGetterField;

    #endregion

    #region Methods

    /// <summary>
    /// Gets the field value from the supplied item.
    /// </summary>
    /// <param name="item">Source item.</param>
    /// <returns>Field value.</returns>
    public virtual object GetValue( TItem item )
    {
        if ( item is null || string.IsNullOrWhiteSpace( Field ) )
            return null;

        if ( valueGetter is null || !string.Equals( valueGetterField, Field, StringComparison.Ordinal ) )
        {
            valueGetterField = Field;
            valueGetter = PivotGridFunctionCompiler.CreateValueGetter<TItem>( Field );
        }

        return valueGetter.Invoke( item );
    }

    /// <summary>
    /// Formats a field value.
    /// </summary>
    /// <param name="value">Field value.</param>
    /// <returns>Formatted field value.</returns>
    public virtual string FormatValue( object value )
    {
        if ( DisplayFormat is not null )
        {
            return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
        }

        return value?.ToString() ?? EmptyText;
    }

    /// <summary>
    /// Gets the field caption.
    /// </summary>
    /// <returns>Field caption.</returns>
    public string GetCaption()
        => string.IsNullOrWhiteSpace( Caption ) ? Field : Caption;

    #endregion

    #region Properties

    /// <summary>
    /// Field path bound to this PivotGrid field.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Optional caption shown in headers.
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Specifies display format for default display text.
    /// </summary>
    public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies display format provider.
    /// </summary>
    public IFormatProvider DisplayFormatProvider { get; set; }

    /// <summary>
    /// Text shown when the field value is null.
    /// </summary>
    public string EmptyText { get; set; } = string.Empty;

    /// <summary>
    /// Defines custom header template.
    /// </summary>
    public RenderFragment<PivotGridHeaderContext<TItem>> HeaderTemplate { get; set; }

    /// <summary>
    /// Defines custom field value template.
    /// </summary>
    public RenderFragment<PivotGridFieldValueContext<TItem>> DisplayTemplate { get; set; }

    #endregion
}