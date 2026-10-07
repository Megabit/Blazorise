#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays an actionable menu entry inside a ribbon dropdown or split button.
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
        if ( firstRender )
        {
            ElementRef = itemRef.ElementRef;
        }

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
    /// Specifies the menu command label displayed when <see cref="ChildContent"/> is not supplied.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Specifies the icon displayed beside the label. Accepts an icon name supported by the configured icon provider.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Prevents selecting the menu command while keeping it visible. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Highlights the menu command as active without invoking it. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Closes the containing dropdown and its ancestor dropdowns when the command is selected.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool CloseParentDropdowns { get; set; } = true;

    /// <summary>
    /// Occurs when the enabled menu command is selected.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Defines custom menu command content, replacing the default icon and label.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}