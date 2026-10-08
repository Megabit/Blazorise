#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Opens a menu of related ribbon commands from an icon and label.
/// </summary>
public partial class RibbonDropdown : BaseRibbonItem
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
    /// Defines custom content for the menu trigger, replacing its default icon and label.
    /// </summary>
    [Parameter] public RenderFragment ButtonContent { get; set; }

    #endregion
}