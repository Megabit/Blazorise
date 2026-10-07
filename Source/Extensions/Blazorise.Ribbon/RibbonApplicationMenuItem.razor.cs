#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays an application menu command or a submenu containing nested items.
/// </summary>
public partial class RibbonApplicationMenuItem : BaseComponent
{
    #region Members

    private DropdownToggle toggleRef;

    private DropdownItem itemRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = HasSubmenu ? toggleRef.ElementRef : itemRef.ElementRef;

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles activation of a leaf command received from the dropdown item.
    /// </summary>
    /// <param name="value">
    /// The value supplied by the dropdown item.
    /// </param>
    protected Task OnClickHandler( object value ) => Disabled ? Task.CompletedTask : HandleClick();

    /// <summary>
    /// Invokes the application command.
    /// </summary>
    protected virtual Task HandleClick() => Clicked.InvokeAsync();

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Indicates whether child content makes this entry a submenu instead of a command.
    /// </summary>
    protected bool HasSubmenu => ChildContent is not null;

    /// <summary>
    /// Specifies the label of the application command or submenu.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the icon displayed beside the label. Accepts an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Prevents activating the command or opening its submenu. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Occurs when an enabled command without children is selected. Entries with children open their submenu instead.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Defines the submenu content.
    /// </summary>
    /// <remarks>
    /// Use nested <see cref="RibbonApplicationMenuItem"/> components to define submenu commands.
    /// When supplied, this entry opens a submenu instead of invoking its click callback.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}