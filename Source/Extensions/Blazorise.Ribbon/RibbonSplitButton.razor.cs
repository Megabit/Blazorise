#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Separates a default action from a menu of related actions.
/// </summary>
public partial class RibbonSplitButton : BaseRibbonItem
{
    #region Members

    private Dropdown dropdownRef;

    private DropdownToggle toggleRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = dropdownRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles activation of the default action.
    /// </summary>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
        => Disabled ? Task.CompletedTask : HandleClick( eventArgs );

    /// <summary>
    /// Invokes the default action.
    /// </summary>
    protected virtual Task HandleClick( MouseEventArgs eventArgs ) => Clicked.InvokeAsync( eventArgs );

    /// <summary>
    /// Handles visibility changes received from the dropdown.
    /// </summary>
    protected Task OnVisibleChangedHandler( bool visible ) => HandleVisibleChanged( visible );

    /// <summary>
    /// Updates menu visibility before notifying the application.
    /// </summary>
    protected virtual async Task HandleVisibleChanged( bool visible )
    {
        if ( Visible == visible )
        {
            return;
        }

        Visible = visible;

        await VisibleChanged.InvokeAsync( visible );
    }

    /// <summary>
    /// Opens the menu when the ribbon command is enabled.
    /// </summary>
    public Task Show() => Disabled ? Task.CompletedTask : dropdownRef.Show();

    /// <summary>
    /// Closes the menu.
    /// </summary>
    public Task Hide() => dropdownRef.Hide();

    /// <summary>
    /// Moves keyboard focus to the menu toggle.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the toggle into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => toggleRef.Focus( scrollToElement );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Uses <see cref="MenuLabel"/> as the menu toggle accessible name, falling back to the command text.
    /// </summary>
    protected string EffectiveMenuLabel => MenuLabel ?? Text;

    /// <summary>
    /// Specifies the accessible name of the menu toggle. When omitted, uses <see cref="BaseRibbonItem.Text"/>.
    /// </summary>
    [Parameter] public string MenuLabel { get; set; }

    /// <summary>
    /// Controls whether the command's menu is open. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when the menu opens or closes. Supplies its visibility for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Controls the direction in which the menu opens. Defaults to <see cref="Direction.Down"/>.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Occurs when the default action button is activated. Activating the menu toggle opens the menu instead.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Defines custom content for the default action button, replacing its default icon and label.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    #endregion
}