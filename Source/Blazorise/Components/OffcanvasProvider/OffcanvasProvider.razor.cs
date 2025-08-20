using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Components.OffcanvasProvider;
public partial class OffcanvasProvider : BaseComponent
{
    #region Members

    private List<OffcanvasInstance> offCanvasInstances;

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
        var newOffCanvasInstance = new OffcanvasInstance( this, IdGenerator.Generate, title, childContent, offcanvasInstanceOptions );

        return Show( newOffCanvasInstance );
    }

    internal async Task<OffcanvasInstance> Show( OffcanvasInstance offcanvasInstance )
    {
        offCanvasInstances ??= new();

        var existingOffCanvasInstance = offCanvasInstances.FirstOrDefault( x => x.OffCanvasId == offcanvasInstance.OffCanvasId );

        if ( existingOffCanvasInstance is null && !offCanvasInstances.Contains( offcanvasInstance ) )
        {
            offCanvasInstances.Add( offcanvasInstance );
        }

        await InvokeAsync( StateHasChanged );

        //HACK:: This is a workaround to ensure the offCanvas is rendered before showing it.
        await Task.Delay( 10 );

        var currentOffCanvasInstance = existingOffCanvasInstance ?? offcanvasInstance;

        await currentOffCanvasInstance.OffCanvasRef.Show();

        return existingOffCanvasInstance ?? offcanvasInstance;
    }

    /// <summary>
    /// Closes currently opened offCanvas.
    /// </summary>
    /// <returns></returns>
    internal Task Hide()
        => offCanvasInstances?.LastOrDefault()?.OffCanvasRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Closes the offCanvas.
    /// </summary>
    /// <returns></returns>
    internal Task Hide( OffcanvasInstance offcanvasInstance )
        => offCanvasInstances?.FirstOrDefault( x => x.IsEqual( offcanvasInstance ) )?.OffCanvasRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Returns all the offCanvas instances.
    /// </summary>
    internal IEnumerable<OffcanvasInstance> GetInstances()
        => offCanvasInstances;

    /// <summary>
    /// Resets the state of the OffCanvasProvider.
    /// Any existing instances will be cleared.
    /// </summary>
    /// <returns></returns>
    internal async Task Reset()
    {
        offCanvasInstances = null;

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Explicitly removes the offCanvas instance from the OffCanvasProvider.
    /// </summary>
    /// <param name="offcanvasInstance">The offCanvas instance</param>
    /// <returns></returns>
    internal async Task Remove( OffcanvasInstance offcanvasInstance )
    {
        if ( offCanvasInstances.IsNullOrEmpty() )
        {
            return;
        }

        offCanvasInstances.Remove( offcanvasInstance );

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handles the closing of the offCanvas.
    /// </summary>
    /// <returns></returns>
    protected async Task OnOffCanvasClosed( OffcanvasInstance offcanvasInstance )
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
    /// The SlateOffCanvasService
    /// </summary>
    [Inject] protected IOffcanvasService OffcanvasService { get; set; }

    /// <summary>
    /// Keeps the OffCanvasInstance in memory after it has been closed.
    /// Defaults to false.
    /// </summary>
    [Parameter] public bool Stateful { get; set; }

    [Parameter] public bool ShowCloseButton { get; set; }
    [Parameter] public EventCallback CloseButtonClicked { get; set; }

    /// <summary>
    /// Uses the offCanvas standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// Global Option.
    /// </summary>
    [Parameter] public bool UseOffCanvasStructure { get; set; }

    [Parameter] public Func<OffcanvasOpeningEventArgs, Task> Opening { get; set; }
    [Parameter] public EventCallback Opened { get; set; }
    [Parameter] public Func<OffcanvasClosingEventArgs, Task> Closing { get; set; }
    [Parameter] public EventCallback Closed { get; set; }
    [Parameter] public bool ShowBackdrop { get; set; } = true;
    [Parameter] public bool Animated { get; set; } = true;
    [Parameter] public int AnimationDuration { get; set; } = 150;
    [Parameter] public Placement Placement { get; set; } = Placement.End;

    #endregion
}
