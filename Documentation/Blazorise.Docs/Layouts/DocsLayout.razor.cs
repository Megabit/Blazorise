#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Docs.Services.Search;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Layouts;

public partial class DocsLayout
{
    #region Members

    private Bar sideBar;

    private Bar topBar;

    private bool topBarVisible;

    private bool sideBarGuidesMenuVisible;

    private bool sideBarUiProvidersMenuVisible;

    private bool sideBarComponentsMenuVisible;

    private bool sideBarDataGridMenuVisible;

    private bool sideBarSvgChartMenuVisible;

    private bool sideBarServicesMenuVisible;

    private bool sideBarSpecificationsMenuVisible;

    private bool sideBarExtensionsMenuVisible;

    private bool sideBarHelpersMenuVisible;

    private bool sideBarMetaMenuVisible;

    public string selectedSearchText { get; set; }

    private bool isRouterTabsExample;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        await SearchMenuProvider.InitializeAsync(); //TODO: this should be done in an IHostedService.StartAsync

        await base.OnInitializedAsync();
    }

    private async void OnLocationChanged( object sender, LocationChangedEventArgs e )
    {
        var isRouterTabsPage = e.Location.Contains( "routertabs" );
        if ( isRouterTabsExample != isRouterTabsPage )
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

    private bool IsGuidesMenuRoute( string currentUri )
    {
        var relativePath = NavigationManager.ToBaseRelativePath( currentUri )
            .Split( '?', '#' )[0]
            .TrimEnd( '/' );

        return relativePath.Equals( "docs", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/start", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/templates", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/testing", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/theming", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/analyzer", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/migration", StringComparison.OrdinalIgnoreCase )
               || relativePath.Equals( "docs/usage/licensing/register-product-token", StringComparison.OrdinalIgnoreCase );
    }

    private bool IsUiProvidersMenuRoute( string currentUri )
    {
        var relativePath = NavigationManager.ToBaseRelativePath( currentUri )
            .Split( '?', '#' )[0]
            .TrimEnd( '/' );

        return relativePath.Equals( "docs/usage", StringComparison.OrdinalIgnoreCase )
               || ( relativePath.StartsWith( "docs/usage/", StringComparison.OrdinalIgnoreCase )
                    && !relativePath.StartsWith( "docs/usage/licensing", StringComparison.OrdinalIgnoreCase ) );
    }

    private bool IsExtensionsMenuRoute( string currentUri )
    {
        var relativePath = NavigationManager.ToBaseRelativePath( currentUri )
            .Split( '?', '#' )[0]
            .TrimEnd( '/' );

        return relativePath.Equals( "docs/extensions", StringComparison.OrdinalIgnoreCase )
               || ( relativePath.StartsWith( "docs/extensions/", StringComparison.OrdinalIgnoreCase )
                    && !relativePath.StartsWith( "docs/extensions/datagrid", StringComparison.OrdinalIgnoreCase )
                    && !relativePath.StartsWith( "docs/extensions/svg-chart", StringComparison.OrdinalIgnoreCase ) );
    }

    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    [Inject] public SearchEntriesProvider SearchMenuProvider { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    #endregion
}