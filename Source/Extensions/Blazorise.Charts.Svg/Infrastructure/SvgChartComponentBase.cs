#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Base class for native SVG chart child configuration components.
/// </summary>
public abstract class SvgChartComponentBase : BaseStyledComponent, System.IDisposable
{
    #region Members

    private SvgChartBase registeredParent;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var shouldRefresh = parameters.IsParameterChanged( Class )
            || parameters.IsParameterChanged( Style );

        await base.SetParametersAsync( parameters );

        if ( shouldRefresh )
        {
            RegisteredParent?.Refresh();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Register();
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing && registeredParent is not null )
        {
            Unregister();
            registeredParent = null;
        }

        base.Dispose( disposing );
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

    #endregion

    #region Properties

    internal SvgChartBase RegisteredParent => registeredParent;

    [CascadingParameter] internal SvgChartBase Parent { get; set; }

    #endregion
}