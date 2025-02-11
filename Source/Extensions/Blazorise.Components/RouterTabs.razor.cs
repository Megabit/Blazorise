#region Using directives
using System;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise.Components;

/// <summary>
/// A component that manages router-based tab navigation.
/// </summary>
public partial class RouterTabs : ComponentBase, IDisposable
{
    #region Members

    private bool disposedValue;

    #endregion

    #region Methods

    /// <inheritdoc/>
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

    /// <summary>
    /// Closes a specified router tab.
    /// </summary>
    /// <param name="routerTabsItem">The tab item to close.</param>
    internal void CloseTab( RouterTabsItem routerTabsItem )
    {
        RouterTabsService?.CloseRouterTab( routerTabsItem );
    }

    /// <summary>
    /// Handles navigation location changes and updates the router tabs accordingly.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The event arguments containing location change details.</param>
    private void OnLocationChanged( object sender, LocationChangedEventArgs args )
    {
        RouterTabsService?.TrySetRouteData( RouteData );
    }

    /// <summary>
    /// Invokes a state change when the router tabs service state is updated.
    /// </summary>
    private void OnStateHasChanged()
    {
        InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Gets the localized name of a specified tab.
    /// </summary>
    /// <param name="tab">The tab item for which to retrieve the localized name.</param>
    /// <returns>The localized name of the tab.</returns>
    private string GetTabName( RouterTabsItem tab )
    {
        return NameLocalizer?.Invoke( tab.Name ) ?? tab.LocalizedNameOrName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides access to the navigation manager for handling routing events.
    /// </summary>
    [Inject] public NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Provides access to the router tabs service for managing tab state.
    /// </summary>
    [Inject] public RouterTabsService RouterTabsService { get; set; }

    /// <summary>
    /// Provides the current route data for the component.
    /// </summary>
    [CascadingParameter] public RouteData RouteData { get; set; }

    /// <summary>
    /// A function used to localize tab names.
    /// </summary>
    [Parameter] public TextLocalizerHandler NameLocalizer { get; set; }

    #endregion
}