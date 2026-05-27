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

    protected override void OnInitialized()
    {
        Register();
    }

    protected abstract void Register();

    protected abstract void Unregister();

    protected void SetRegisteredParent()
    {
        registeredParent = Parent;
        registeredParent?.Refresh();
    }

    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
    }

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