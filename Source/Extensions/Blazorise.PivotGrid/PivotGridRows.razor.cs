#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Container for declarative PivotGrid rows.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class PivotGridRows<TItem>
{
    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }

    /// <summary>
    /// Child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}