#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// A modal provider to be set at the root of your app, providing a programmatic way to invoke modals with custom content by using <see cref="ModalService"/>.
/// </summary>
public partial class ModalProvider : BaseComponent
{
    #region Members

    private List<ModalInstance> modalInstances;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        ModalService.SetModalProvider( this );

        return base.OnInitializedAsync();
    }

    internal async Task<ModalInstance> Show( string title, RenderFragment childContent, ModalInstanceOptions modalInstanceOptions )
    {
        modalInstances ??= new();

        var newModalInstance = new ModalInstance( this, IdGenerator.Generate, title, childContent, modalInstanceOptions );
        modalInstances.Add( newModalInstance );

        await InvokeAsync( StateHasChanged );
        return newModalInstance;
    }

    /// <summary>
    /// Closes currently opened modal.
    /// </summary>
    /// <returns></returns>
    internal Task Hide()
        => modalInstances?.LastOrDefault()?.ModalRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Closes the modal.
    /// </summary>
    /// <returns></returns>
    internal Task Hide( ModalInstance modalInstance )
        => modalInstances?.FirstOrDefault( x => x.IsEqual( modalInstance ) )?.ModalRef?.Hide() ?? Task.CompletedTask;

    /// <summary>
    /// Handles the closing of the modal.
    /// </summary>
    /// <returns></returns>
    protected async Task OnModalClosed( ModalInstance modalInstance )
    {
        await modalInstance.Closed.InvokeAsync();
        modalInstances.Remove( modalInstance );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The ModalService
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; }

    /// <summary>
    /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
    /// Defaults to true.
    /// Global Option.
    /// </summary>
    [Parameter] public bool UseModalStructure { get; set; } = true;

    /// <summary>
    /// If true modal will scroll to top when opened.
    /// Global Option.
    /// </summary>
    [Parameter] public bool ScrollToTop { get; set; } = true;

    /// <summary>
    /// Occurs before the modal is opened.
    /// Global Option.
    /// </summary>
    [Parameter] public Func<ModalOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Occurs before the modal is closed.
    /// Global Option.
    /// </summary>
    [Parameter] public Func<ModalClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Occurs after the modal has opened.
    /// Global Option.
    /// </summary>
    [Parameter] public EventCallback Opened { get; set; }

    /// <summary>
    /// Occurs after the modal has closed.
    /// Global Option.
    /// </summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <summary>
    /// Specifies the backdrop needs to be rendered for this <see cref="Modal"/>.
    /// Global Option.
    /// </summary>
    [Parameter] public bool ShowBackdrop { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// Global Option.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation duration.
    /// Global Option.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 150;

    /// <summary>
    /// Defines how the modal content will be rendered.
    /// Global Option.
    /// </summary>
    [Parameter] public ModalRenderMode RenderMode { get; set; }

    /// <summary>
    /// Defines if the modal should keep the input focus at all times.
    /// Global Option.
    /// </summary>
    [Parameter] public bool? FocusTrap { get; set; }

    /// <summary>
    /// Centers the modal vertically.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
    /// </remarks>
    [Parameter] public bool Centered { get; set; }

    /// <summary>
    /// Scrolls the modal content independent of the page itself.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
    /// </remarks>
    [Parameter] public bool Scrollable { get; set; }

    /// <summary>
    /// Changes the size of the modal.
    /// </summary>
    /// <remarks>
    /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
    /// </remarks>
    [Parameter] public ModalSize Size { get; set; }

    #endregion
}