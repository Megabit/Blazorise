#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays an application command with the appearance and alignment of a ribbon tab heading.
/// </summary>
public partial class RibbonApplicationButton : BaseComponent
{
    #region Members

    private Button buttonRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the utilities matching an unselected ribbon tab heading.
    /// </summary>
    public RibbonApplicationButton()
    {
        Border = Blazorise.Border.Is0;
        Background = Blazorise.Background.Transparent;
        Flex = Blazorise.Flex.Shrink.Is0;
        TextOverflow = Blazorise.TextOverflow.NoWrap;
        TextSize = Blazorise.TextSize.Px( 13 );
        Padding = Blazorise.Padding.Is0;
        TextColor = Blazorise.TextColor.Default;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        ElementRef = buttonRef.ElementRef;

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Handles activation of the application command.
    /// </summary>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
        => Disabled ? Task.CompletedTask : HandleClick( eventArgs );

    /// <summary>
    /// Invokes the application command without changing ribbon selection.
    /// </summary>
    protected virtual Task HandleClick( MouseEventArgs eventArgs ) => Clicked.InvokeAsync( eventArgs );

    /// <summary>
    /// Moves keyboard focus to the application button.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the button into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => buttonRef.Focus( scrollToElement );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the expanded state for accessibility markup, or null for a command without expandable content.
    /// </summary>
    protected string ExpandedString => Expanded switch
    {
        true => "true",
        false => "false",
        _ => null,
    };

    /// <summary>
    /// Gets or sets the application command text and accessible name.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets whether the application command is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the associated menu or backstage is expanded. Leave unset for a command without expandable content.
    /// </summary>
    [Parameter] public bool? Expanded { get; set; }

    /// <summary>
    /// Occurs when the application command is activated.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Gets or sets content replacing the default application command text.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}