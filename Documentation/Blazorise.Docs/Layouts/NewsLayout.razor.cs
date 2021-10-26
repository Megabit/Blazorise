#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Layouts
{
    public partial class NewsLayout : IDisposable
    {
        #region Members

        private bool topBarVisible;

        private bool disposed;

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

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        #endregion
    }
}
