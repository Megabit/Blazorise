using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise;

public partial class TransitionableRoute : ComponentBase
{
    #region Members

    private bool active = true;

    private RouteData lastRouteData;

    private bool invokesStateChanged = true;

    #endregion

    #region Methods

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSModule.Initialize( DotNetObjectReference.Create( this ), new
            {
                Active = active
            } );

            await Navigate( backwards: false, firstRender: true );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    internal async Task Navigate( bool backwards, bool firstRender )
    {
        var routeDataToUse = active ? RouteData : lastRouteData;

        var switchedRouteData = ( active ? lastRouteData : RouteData ) ?? RouteData;

        Transition = RouteTransition.Create( routeDataToUse, switchedRouteData, active, backwards, firstRender );

        if ( invokesStateChanged )
        {
            await InvokeAsync( StateHasChanged );
        }

        var canResetStateOnTransitionOut = ForgetStateOnTransition && !active;

        await Task.Yield();

        if ( active )
        {
            await TransitionInvoker.InvokeRouteTransitionAsync( Transition );
        }

        active = !active;
        lastRouteData = RouteData;

        if ( canResetStateOnTransitionOut )
        {
            Transition = RouteTransition.Create( routeData: null, switchedRouteData: null, Transition.IntoView, Transition.Backwards, Transition.FirstRender );

            await Task.Delay( TransitionDurationMilliseconds );

            if ( invokesStateChanged )
            {
                await InvokeAsync( StateHasChanged );
            }
        }
    }

    [JSInvokable]
    public async Task Navigate( bool backwards )
    {
        await Navigate( backwards, firstRender: false );
    }

    protected void MakeSecondary()
    {
        active = false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="IJSTransitionableRouteModule"/> instance.
    /// </summary>
    [Inject] public IJSTransitionableRouteModule JSModule { get; set; }

    [Inject] IRouteTransitionInvoker TransitionInvoker { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public RouteData RouteData { get; set; }

    [Parameter] public RouteTransition Transition { get; set; }

    [Parameter] public bool ForgetStateOnTransition { get; set; } = false;

    [Parameter] public int TransitionDurationMilliseconds { get; set; } = 1000;

    #endregion
}