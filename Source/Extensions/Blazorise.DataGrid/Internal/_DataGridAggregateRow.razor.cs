#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

public partial class _DataGridAggregateRow<TItem> : BaseDataGridComponent
{
    #region Methods

    protected object Calculate( DataGridAggregate<TItem> aggregate, DataGridColumn<TItem> column )
        => aggregate?.AggregationFunction?.Invoke( Data, column );

    #endregion

    #region Properties

    protected IEnumerable<TItem> Data
        => ParentDataGrid.ManualReadMode ? ParentDataGrid.AggregateData : ParentDataGrid.FilteredData;

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// List of aggregate columns used to build this row.
    /// </summary>
    [Parameter] public IEnumerable<DataGridAggregate<TItem>> Aggregates { get; set; }

    /// <summary>
    /// Custom css classname.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Custom html style.
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Custom background.
    /// </summary>
    [Parameter] public Background Background { get; set; }

    /// <summary>
    /// Custom color.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}