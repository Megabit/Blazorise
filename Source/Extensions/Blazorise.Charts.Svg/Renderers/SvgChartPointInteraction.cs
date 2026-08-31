#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartPointInteraction
{
    #region Members

    private readonly SvgChartPluginRenderContext chart;

    private SvgChartPointEventArgs point;

    private string color;

    #endregion

    #region Constructors

    public SvgChartPointInteraction( SvgChartPluginRenderContext chart )
    {
        this.chart = chart;
        Clicked = EventCallback.Factory.Create<MouseEventArgs>( chart.EventReceiver, HandleClicked );
        MouseEntered = EventCallback.Factory.Create<MouseEventArgs>( chart.EventReceiver, HandleMouseEntered );
        MouseLeft = EventCallback.Factory.Create<MouseEventArgs>( chart.EventReceiver, HandlePointLeft );
        Focused = EventCallback.Factory.Create<FocusEventArgs>( chart.EventReceiver, HandleFocused );
        Blurred = EventCallback.Factory.Create<FocusEventArgs>( chart.EventReceiver, HandlePointLeft );
    }

    #endregion

    #region Methods

    public void Update( SvgChartPointEventArgs point, string color )
    {
        this.point = point;
        this.color = color;
    }

    private Task HandleClicked( MouseEventArgs eventArgs )
    {
        return chart.NotifyPointClicked( point, color );
    }

    private Task HandleMouseEntered( MouseEventArgs eventArgs )
    {
        return chart.NotifyPointHovered( point, color );
    }

    private void HandlePointLeft()
    {
        chart.NotifyPointLeft();
    }

    private void HandleFocused( FocusEventArgs eventArgs )
    {
        chart.ShowTooltip( point, color, false );
    }

    #endregion

    #region Properties

    public EventCallback<MouseEventArgs> Clicked { get; }

    public EventCallback<MouseEventArgs> MouseEntered { get; }

    public EventCallback<MouseEventArgs> MouseLeft { get; }

    public EventCallback<FocusEventArgs> Focused { get; }

    public EventCallback<FocusEventArgs> Blurred { get; }

    #endregion
}