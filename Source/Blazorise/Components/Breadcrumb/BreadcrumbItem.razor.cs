#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for a breadcrumb link.
/// </summary>
public partial class BreadcrumbItem : BaseComponent, IDisposable
{
    #region Members

    private bool active;

    private string absoluteUri;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BreadcrumbItem() );
        builder.Append( ClassProvider.BreadcrumbItemActive( Active ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose( disposing );
    }

    /// <summary>
    /// Handles the navigation change event.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs">Location changed arguments.</param>
    protected virtual void OnLocationChanged( object sender, LocationChangedEventArgs eventArgs )
    {
        if ( ParentBreadcrumb?.Mode == BreadcrumbMode.Auto )
        {
            Active = eventArgs.Location == absoluteUri;

            InvokeAsync( StateHasChanged );
        }
    }

    /// <summary>
    /// Notify us that the link relative address has changed.
    /// </summary>
    /// <param name="relativeUri"></param>
    internal void NotifyRelativeUriChanged( string relativeUri )
    {
        // uri will always be applied, no matter the BreadcrumbActivation state.
        absoluteUri = relativeUri is null ? string.Empty : NavigationManager.ToAbsoluteUri( relativeUri ).AbsoluteUri;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether this breadcrumb item is the active (current) item.
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

    /// <summary>
    /// Provides access to the navigation manager for handling navigation and URI management.
    /// </summary>
    [Inject] private NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Specifies the content to render inside this <see cref="BreadcrumbItem"/> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// A reference to the parent <see cref="Breadcrumb"/> component that contains this breadcrumb item.
    /// </summary>
    /// <remarks>
    /// This cascading parameter is used to inherit context and manage the behavior of the breadcrumb item, such as automatic active state detection.
    /// </remarks>
    [CascadingParameter] protected Breadcrumb ParentBreadcrumb { get; set; }

    #endregion
}