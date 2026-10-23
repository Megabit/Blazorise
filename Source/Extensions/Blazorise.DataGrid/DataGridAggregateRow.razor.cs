#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Groups aggregates into a single DataGrid summary row.
/// </summary>
public partial class DataGridAggregateRow : ComponentBase
{
    #region Properties

    /// <summary>
    /// Aggregate definitions displayed in this row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}