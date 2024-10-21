#region Using directives
using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise.Components;

/// <summary>
/// Component that manages the router tabs.
/// </summary>
public partial class RouterTabs : ComponentBase, IDisposable
{
    #region Members

    private bool disposedValue;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( NavigationManager is not null )
            NavigationManager.LocationChanged += OnLocationChanged;

        if ( RouterTabsService is not null )
            RouterTabsService.StateHasChanged += OnStateHasChanged;

        if ( RouteData is not null )
            RouterTabsService.TrySetRouteData( RouteData );
    }

    internal void CloseTab( RouterTabsItem routerTabsItem )
    {
        if ( RouterTabsService is not null )
            RouterTabsService.CloseRouterTab( routerTabsItem );
    }

    private void OnLocationChanged( object o, LocationChangedEventArgs _ )
    {
        if ( RouterTabsService is not null )
            RouterTabsService.TrySetRouteData( RouteData );
    }

    private void OnStateHasChanged()
    {
        InvokeAsync( StateHasChanged );
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposedValue )
        {
            if ( disposing )
            {
                if ( RouterTabsService is not null )
                {
                    RouterTabsService.StateHasChanged -= OnStateHasChanged;
                    RouterTabsService.Clear();
                }

                if ( NavigationManager is not null )
                    NavigationManager.LocationChanged -= OnLocationChanged;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }

    #endregion

    #region Properties

    [Inject] public NavigationManager NavigationManager { get; set; }

    [Inject] public RouterTabsService RouterTabsService { get; set; }

    [CascadingParameter] public RouteData RouteData { get; set; }

    #endregion
}