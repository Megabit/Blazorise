using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Docs.Components
{
    public partial class Toc : BaseAfterRenderComponent
    {
        ElementReference elementRef;

        protected override void OnInitialized()
        {
            if ( NavigationManager != null )
            {
                NavigationManager.LocationChanged += NavigationManager_LocationChanged;

                ExecuteAfterRender( async () =>
                {
                    await JSRuntime.InvokeVoidAsync( "blazoriseDocs.navigation.generateToc", elementRef, new
                    {
                        BasePath = NavigationManager.Uri
                    } );
                } );
            }

            base.OnInitialized();
        }

        private void NavigationManager_LocationChanged( object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e )
        {
            InvokeAsync( async () =>
            {
                await JSRuntime.InvokeVoidAsync( "blazoriseDocs.navigation.generateToc", elementRef, new
                {
                    BasePath = NavigationManager.Uri
                } );
            } );
        }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; }
    }
}
