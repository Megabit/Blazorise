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

namespace Blazorise.Docs.Layouts
{
    public partial class DocsLayout : IDisposable
    {
        #region Members

        private Bar sideBar;

        private Bar topBar;

        private bool topBarVisible;

        private bool sideBarFormMenuVisible;

        private bool sideBarComponentsMenuVisible;

        private bool sideBarServicesMenuVisible;

        private bool sideBarExtensionsMenuVisible;

        private bool sideBarHelpersMenuVisible;

        private bool sideBarMetaMenuVisible;

        private bool disposed;

        public IEnumerable<SearchEntry> SearchEntries;

        public string selectedSearchText { get; set; }

        [Inject]
        public SearchEntryData SearchEntryData { get; set; }

        #endregion

        #region Methods

        protected override Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            SearchEntries = SearchEntryData.GetDataAsync().Result;

            return base.OnInitializedAsync();
        }

        private async void ComponentSearchSelectedValueChanged( string value )
        {
            if ( !string.IsNullOrWhiteSpace( value ) )
            {
                NavigationManager.NavigateTo( value );
                await Task.Delay( 1000 );
                selectedSearchText = "";
                StateHasChanged();
            }
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
