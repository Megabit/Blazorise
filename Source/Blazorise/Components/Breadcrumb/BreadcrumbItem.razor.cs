#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise
{
    public partial class BreadcrumbItem : BaseComponent
    {
        #region Members

        private bool active;

        private string absoluteUri;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BreadcrumbItem() );
            builder.Append( ClassProvider.BreadcrumbItemActive(), Active );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                if ( ParentBreadcrumb?.Mode == BreadcrumbMode.Auto && absoluteUri == NavigationManager.Uri )
                {
                    Active = true;

                    await InvokeAsync( StateHasChanged );
                }
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                NavigationManager.LocationChanged -= OnLocationChanged;
            }

            base.Dispose( disposing );
        }

        private void OnLocationChanged( object sender, LocationChangedEventArgs args )
        {
            if ( ParentBreadcrumb?.Mode == BreadcrumbMode.Auto )
            {
                Active = args.Location == absoluteUri;

                InvokeAsync( StateHasChanged );
            }
        }

        internal void NotifyRelativeUriChanged( string relativeUri )
        {
            // uri will always be applied, no matter the BreadcrumbActivation state.
            absoluteUri = relativeUri == null ? string.Empty : NavigationManager.ToAbsoluteUri( relativeUri ).AbsoluteUri;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item active state.
        /// </summary>
        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                active = value;

                DirtyClasses();
            }
        }

        [Inject] private NavigationManager NavigationManager { get; set; }

        [CascadingParameter] protected Breadcrumb ParentBreadcrumb { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
