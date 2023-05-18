using Microsoft.AspNetCore.Components;

namespace Blazorise;

public class RouteTransition
{
    private RouteTransition( RouteData routeData, RouteData switchedRouteData, bool intoView, bool backwards, bool firstRender )
    {
        this.RouteData = routeData;
        this.SwitchedRouteData = switchedRouteData;
        this.IntoView = intoView;
        this.Backwards = backwards;
        this.FirstRender = firstRender;
    }

    public static RouteTransition Create( RouteData routeData, RouteData switchedRouteData, bool intoView, bool backwards, bool firstRender )
        => new RouteTransition( routeData, switchedRouteData, intoView, backwards, firstRender );

    public RouteData RouteData { get; }

    public RouteData SwitchedRouteData { get; }

    /// <summary>
    /// If true, the route should be made visible.
    /// </summary>
    public bool IntoView { get; }

    /// <summary>
    /// If true, the reverse animations will be applied.
    /// </summary>
    public bool Backwards { get; }

    /// <summary>
    ///  Stops transitioning on first load.
    /// </summary>
    public bool FirstRender { get; }
}
