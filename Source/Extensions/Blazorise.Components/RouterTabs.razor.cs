#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise.Components;

public partial class RouterTabs : ComponentBase, IDisposable
{
    private bool disposedValue;

    [CascadingParameter] public RouteData RouteData { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public RouterTabsService RouterTabsService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        NavigationManager.LocationChanged += OnLocationChanged;
        RouterTabsService.OnStateHasChanged += OnStateHasChanged;
        if ( RouteData  is not null)
            RouterTabsService.TrySetRouteData( RouteData );
    }

    internal void CloseTab(RouterTabsItem routerTabsItem)
    {
        RouterTabsService.CloseRouterTab( routerTabsItem );
    }

    private void OnLocationChanged( object o, LocationChangedEventArgs _ )
    {
        RouterTabsService.TrySetRouteData( RouteData );
    }

    private void OnStateHasChanged()
    {
        StateHasChanged();
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposedValue )
        {
            if ( disposing )
            {
                RouterTabsService.OnStateHasChanged -= OnStateHasChanged;
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
}