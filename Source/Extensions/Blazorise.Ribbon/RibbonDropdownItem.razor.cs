#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays a ribbon menu command using a provider-native dropdown item.
/// </summary>
public partial class RibbonDropdownItem : BaseComponent
{
    #region Members

    private DropdownItem itemRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = itemRef.ElementRef;

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles activation received from the dropdown item.
    /// </summary>
    /// <param name="value">
    /// The value supplied by the dropdown item.
    /// </param>
    protected Task OnClickHandler( object value ) => Disabled ? Task.CompletedTask : HandleClick();

    /// <summary>
    /// Invokes the ribbon menu command.
    /// </summary>
    protected virtual Task HandleClick() => Clicked.InvokeAsync();

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the command text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Gets or sets whether the command is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the command is active.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Gets or sets whether selecting the command closes the entire dropdown hierarchy.
    /// </summary>
    [Parameter] public bool CloseParentDropdowns { get; set; } = true;

    /// <summary>
    /// Occurs when the command is selected.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Gets or sets content replacing the default icon and text.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}