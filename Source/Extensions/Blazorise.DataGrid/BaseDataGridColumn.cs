#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides base data grid column behavior within a DataGrid.
/// </summary>
[CascadingTypeParameter( nameof( TItem ) )]
public class BaseDataGridColumn<TItem> : BaseDataGridComponent
{
    #region Methods

    /// <summary>
    /// Gets the formatted display value.
    /// </summary>
    /// <param name="value">Item the contains the value to format.</param>
    /// <returns>Formatted display value.</returns>
    public virtual string FormatDisplayValue( object value )
    {
        return Formaters.FormatDisplayValue( value, DisplayFormat, DisplayFormatProvider );
    }

    #endregion

    #region Properties

    /// <summary>
    /// To bind a column to a data source field, set this property to the required data field name.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Specifies a direct format specifier or composite format string for the display value.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Specifies the format provider info for display value.
    /// </summary>
    [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseDataGridColumn{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}