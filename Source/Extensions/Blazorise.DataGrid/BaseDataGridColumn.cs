#region Using directives
using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

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
        if ( DisplayFormat is not null )
        {
            return string.Format( DisplayFormatProvider ?? CultureInfo.CurrentCulture, DisplayFormat, value );
        }

        return value?.ToString();
    }

    #endregion

    #region Properties

    /// <summary>
    /// To bind a column to a data source field, set this property to the required data field name.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Defines the format for display value.
    /// </summary>
    [Parameter] public string DisplayFormat { get; set; }

    /// <summary>
    /// Defines the format provider info for display value.
    /// </summary>
    [Parameter] public IFormatProvider DisplayFormatProvider { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseDataGridColumn{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}