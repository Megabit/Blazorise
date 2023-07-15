using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Docs.Components
{
    public partial class Toc : IDisposable
    {
        ElementReference elementRef;

        protected override void OnInitialized()
        {
            if ( NavigationManager is not null )
            {
                NavigationManager.LocationChanged += OnLocationChanged;
            }

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await GenerateToc();
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        private void OnLocationChanged( object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e )
        {
            InvokeAsync( () => GenerateToc() );
        }

        private async Task GenerateToc()
        {
            await JSRuntime.InvokeVoidAsync( "blazoriseDocs.navigation.generateToc", elementRef, new
            {
                BasePath = NavigationManager.Uri
            } );
        }

        public void Dispose()
        {
            if ( NavigationManager is not null )
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }
        }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; }
    }
}
