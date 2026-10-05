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
    #region Methods

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
    /// Gets whether the item opens a submenu.
    /// </summary>
    protected bool HasSubmenu => ChildContent is not null;

    /// <summary>
    /// Gets or sets the command or submenu text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Gets or sets whether the command or submenu is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Occurs when a leaf command is selected. Items with children open their submenu instead.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Gets or sets nested items and other submenu content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}