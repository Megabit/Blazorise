#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// An offcanvas provider to be set at the root of your app, providing a programmatic way to invoke offcanvas panels with custom content by using <see cref="OffcanvasService"/>.
/// </summary>
public partial class OffcanvasProvider : BaseComponent
{
    #region Members

    private List<OffcanvasInstance> offcanvasInstances;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        OffcanvasService.SetOffcanvasProvider( this );

        return base.OnInitializedAsync();
    }

    internal Task<OffcanvasInstance> Show( string title, RenderFragment childContent, OffcanvasInstanceOptions offcanvasInstanceOptions )
    {
        var newOffcanvasInstance = new OffcanvasInstance( this, IdGenerator.Generate, title, childContent, offcanvasInstanceOptions );

        return Show( newOffcanvasInstance );
    }

    internal async Task<OffcanvasInstance> Show( OffcanvasInstance offcanvasInstance )
    {
        offcanvasInstances ??= new();

        var existingOffcanvasInstance = offcanvasInstances.FirstOrDefault( x => x.OffcanvasId == offcanvasInstance.OffcanvasId );

        if ( existingOffcanvasInstance is not null )
        {
            existingOffcanvasInstance.Visible = true;
        }
        else if ( offcanvasInstances.Contains( offcanvasInstance ) )
        {
            offcanvasInstance.Visible = true;
        }
        else
        {
            offcanvasInstance.Visible = true;
            offcanvasInstances.Add( offcanvasInstance );
        }

        await InvokeAsync( StateHasChanged );

        return existingOffcanvasInstance ?? offcanvasInstance;
    }

    /// <summary>
    /// Closes currently opened offcanvas.
    /// </summary>
    internal Task Hide()
        => offcanvasInstances?.LastOrDefault()?.OffcanvasRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Closes the offcanvas.
    /// </summary>
    internal Task Hide( OffcanvasInstance offcanvasInstance )
        => offcanvasInstances?.FirstOrDefault( x => x.IsEqual( offcanvasInstance ) )?.OffcanvasRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Returns all the offcanvas instances.
    /// </summary>
    internal IEnumerable<OffcanvasInstance> GetInstances()
        => offcanvasInstances;

    /// <summary>
    /// Resets the state of the <see cref="OffcanvasProvider"/>.
    /// Any existing instances will be cleared.
    /// </summary>
    internal async Task Reset()
    {
        offcanvasInstances = null;

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Explicitly removes the offcanvas instance from the <see cref="OffcanvasProvider"/>.
    /// </summary>
    /// <param name="offcanvasInstance">The offcanvas instance.</param>
    internal async Task Remove( OffcanvasInstance offcanvasInstance )
    {
        if ( offcanvasInstances.IsNullOrEmpty() )
        {
            return;
        }

        offcanvasInstances.Remove( offcanvasInstance );

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handles the closing of the offcanvas.
    /// </summary>
    protected async Task OnOffcanvasClosed( OffcanvasInstance offcanvasInstance )
    {
        await offcanvasInstance.Closed.InvokeAsync();

        var removeInstance = !offcanvasInstance.Stateful;

        if ( removeInstance )
        {
            await Remove( offcanvasInstance );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="OffcanvasService"/>.
    /// </summary>
    [Inject] protected IOffcanvasService OffcanvasService { get; set; }

    /// <summary>
    /// Uses the offcanvas standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// Global option.
    /// </summary>
    [Parameter] public bool UseOffcanvasStructure { get; set; } = true;

    /// <summary>
    /// Keeps the <see cref="OffcanvasInstance"/> in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    [Parameter] public bool Stateful { get; set; }

    /// <summary>
    /// Shows a close button in the offcanvas header when using the provider structure.
    /// Global option.
    /// </summary>
    [Parameter] public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Callback executed when the close button is clicked.
    /// Global option.
    /// </summary>
    [Parameter] public EventCallback CloseButtonClicked { get; set; }

    /// <summary>
    /// Occurs before the offcanvas is opened.
    /// Global option.
    /// </summary>
    [Parameter] public Func<OffcanvasOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Occurs after the offcanvas has opened.
    /// Global option.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs before the offcanvas is closed.
    /// Global option.
    /// </summary>
    [Parameter] public Func<OffcanvasClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Occurs after the offcanvas has closed.
    /// Global option.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Offcanvas"/>.
    /// Global option.
    /// </summary>
    [Parameter] public bool ShowBackdrop { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// Global option.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation duration.
    /// Global option.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 150;

    /// <summary>
    /// Specifies the position of the offcanvas.
    /// </summary>
    [Parameter] public Placement Placement { get; set; } = Placement.End;

    #endregion
}