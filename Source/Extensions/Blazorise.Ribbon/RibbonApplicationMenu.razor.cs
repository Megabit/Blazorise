#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays an application menu with the appearance and alignment of a ribbon tab heading.
/// </summary>
public partial class RibbonApplicationMenu : BaseComponent
{
    #region Members

    private Dropdown dropdownRef;

    private DropdownToggle toggleRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the utilities matching the application button.
    /// </summary>
    public RibbonApplicationMenu()
    {
        Border = Blazorise.Border.Is0;
        Flex = Blazorise.Flex.Shrink.Is0;
        TextOverflow = Blazorise.TextOverflow.NoWrap;
        TextSize = Blazorise.TextSize.Px( 13 );
        Padding = Blazorise.Padding.Is0;
        Height = Blazorise.Height.Auto;
        TextColor = Blazorise.TextColor.Body;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = toggleRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles visibility changes received from the dropdown.
    /// </summary>
    protected Task OnVisibleChangedHandler( bool visible ) => HandleVisibleChanged( visible );

    /// <summary>
    /// Updates visibility before notifying the application.
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
    /// Opens the application menu when enabled.
    /// </summary>
    public Task Show() => Disabled ? Task.CompletedTask : dropdownRef.Show();

    /// <summary>
    /// Closes the application menu.
    /// </summary>
    public Task Hide() => dropdownRef.Hide();

    /// <summary>
    /// Moves keyboard focus to the application menu trigger.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the trigger into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => toggleRef.Focus( scrollToElement );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the application menu label and accessible name, such as File.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Prevents opening the application menu. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Controls whether the application menu is open. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when the application menu opens or closes. Supplies the open state for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Controls the direction in which the application menu opens. Defaults to <see cref="Direction.Down"/>.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Displays the dropdown indicator beside the application menu heading. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool ShowToggleIcon { get; set; }

    /// <summary>
    /// Defines custom application menu heading content, replacing the visible <see cref="Text"/> label.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Defines the application menu content.
    /// </summary>
    /// <remarks>
    /// Use <see cref="RibbonApplicationMenuItem"/> for commands and <see cref="RibbonApplicationMenuDivider"/> for separators.
    /// Nest menu items to create submenus, such as Save As.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}