using Microsoft.AspNetCore.Components;
using System;

namespace Blazorise;

public class TransitionableLayoutComponent : LayoutComponentBase, ITransitionableLayoutComponent
{
    [CascadingParameter] public RouteTransition Transition { get; set; }

    protected (Type fromType, Type toType) TransitionPageType =>
        (Transition.IntoView ? Transition.SwitchedRouteData.PageType : Transition.RouteData.PageType,
         Transition.IntoView ? Transition.RouteData.PageType : Transition.SwitchedRouteData.PageType);
}