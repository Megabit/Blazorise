#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid.Components;

/// <summary>
/// Internal PivotGrid toolbar renderer.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class _PivotGridToolbar<TItem>
{
    private PivotGridToolbarContext<TItem> Context
        => PivotGrid.CreateToolbarContext();

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }
}