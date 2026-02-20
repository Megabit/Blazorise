#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Layouts;

public partial class BlogLayout
{
    #region Members

    private bool topBarVisible;

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        return base.OnInitializedAsync();
    }

    private async void OnLocationChanged( object sender, LocationChangedEventArgs e )
    {
        await JSRuntime.InvokeVoidAsync( "blazoriseDocs.navigation.scrollToTop" );
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

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    #endregion
}