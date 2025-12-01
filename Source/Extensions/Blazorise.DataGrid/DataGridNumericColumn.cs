#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Datagrid column declarations for numeric types.
/// </summary>
/// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
public class DataGridNumericColumn<TItem> : DataGridColumn<TItem>
{
    /// <inheritdoc/>
    internal override DataGridColumnFilterMethod GetDefaultFilterMethod()
        => DataGridColumnFilterMethod.Equals;

    /// <inheritdoc/>
    public override DataGridColumnType ColumnType
        => DataGridColumnType.Numeric;

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public decimal? Step { get; set; }

    /// <summary>
    /// Maximum number of decimal places after the decimal separator.
    /// </summary>
    [Parameter] public int Decimals { get; set; } = 2;

    /// <summary>
    /// String to use as the decimal separator in numeric values.
    /// </summary>
    [Parameter] public string DecimalSeparator { get; set; } = ".";

    /// <summary>
    /// String to use as the decimal separator in numeric values.
    /// </summary>
    [Parameter] public string GroupSeparator { get; set; } = ",";

    /// <summary>
    /// Helps define the language of an element.
    /// </summary>
    /// <remarks>
    /// https://www.w3schools.com/tags/ref_language_codes.asp
    /// </remarks>
    [Parameter] public string Culture { get; set; }

    /// <summary>
    /// If true, step buttons will be visible.
    /// </summary>
    [Parameter] public bool? ShowStepButtons { get; set; }

    /// <summary>
    /// If true, enables change of numeric value by pressing on step buttons or by keyboard up/down keys.
    /// </summary>
    [Parameter] public bool? EnableStep { get; set; }

    /// <summary>
    /// Renders the native based input <see cref="NumericEdit{TValue}"/> instead of the <see cref="NumericPicker{TValue}"/>.
    /// </summary>
    [Parameter] public bool NativeInputMode { get; set; }
}