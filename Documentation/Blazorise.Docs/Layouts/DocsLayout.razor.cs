#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Shared.Data;
using Blazorise.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Layouts;

public partial class DocsLayout : IDisposable
{
    #region Members

    private Bar sideBar;

    private Bar topBar;

    private bool topBarVisible;

    private bool sideBarIntegrationsMenuVisible;

    private bool sideBarComponentsMenuVisible;

    private bool sideBarServicesMenuVisible;

    private bool sideBarSpecificationsMenuVisible;

    private bool sideBarExtensionsMenuVisible;

    private bool sideBarHelpersMenuVisible;

    private bool sideBarMetaMenuVisible;

    private bool disposed;

    public IEnumerable<PageEntry> SearchEntries;

    public string selectedSearchText { get; set; }

    private bool isRouterTabsExample;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        SearchEntries = await SearchMenuProvider.GetDataAsync();

        await base.OnInitializedAsync();
    }

    private async void OnLocationChanged( object sender, LocationChangedEventArgs e )
    {
        var isRouterTabsPage = e.Location.Contains( "routertabs" );
        if (isRouterTabsExample != isRouterTabsPage )
        {
            isRouterTabsExample = isRouterTabsPage;
            StateHasChanged();
        } 
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.navigation.scrollToTop" );
    }

    private async Task ComponentSearchSelectedValueChanged( string value )
    {
        if ( !string.IsNullOrWhiteSpace( value ) )
        {
            NavigationManager.NavigateTo( value );

            await Task.Delay( 500 );

            selectedSearchText = "";

            await InvokeAsync( StateHasChanged );
        }
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( !disposed )
        {
            if ( disposing )
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    #endregion

    #region Properties

    [Inject] public PageEntryData SearchMenuProvider { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    #endregion
}