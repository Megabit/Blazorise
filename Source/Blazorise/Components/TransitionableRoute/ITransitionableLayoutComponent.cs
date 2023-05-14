using Microsoft.AspNetCore.Components;

namespace Blazorise;

public interface ITransitionableLayoutComponent
{
    /// <summary>
    /// Contains information on the transition behaviour adjust the view.
    /// </summary>
    [CascadingParameter] RouteTransition Transition { get; set; }
}