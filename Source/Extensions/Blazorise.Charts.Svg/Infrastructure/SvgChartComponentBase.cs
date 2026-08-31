#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Base class for native SVG chart child configuration components.
/// </summary>
public abstract class SvgChartComponentBase : ComponentBase, System.IDisposable
{
    #region Members

    private SvgChartBase registeredParent;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Register();
    }

    /// <summary>
    /// Adds this configuration component to its parent chart.
    /// </summary>
    protected abstract void Register();

    /// <summary>
    /// Removes this configuration component from its parent chart.
    /// </summary>
    protected abstract void Unregister();

    /// <summary>
    /// Records that registration completed successfully.
    /// </summary>
    protected void SetRegisteredParent()
    {
        registeredParent = Parent;
        registeredParent?.Refresh();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
    }

    /// <summary>
    /// Unregisters the component from its chart owner.
    /// </summary>
    public void Dispose()
    {
        if ( registeredParent is not null )
        {
            Unregister();
            registeredParent = null;
        }
    }

    #endregion

    #region Properties

    internal SvgChartBase RegisteredParent => registeredParent;

    [CascadingParameter] internal SvgChartBase Parent { get; set; }

    #endregion
}