using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

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
    public bool IntoView { get; }
    public bool Backwards { get; }
    public bool FirstRender { get; }
}
