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
    /// Serializes the expanded state for aria-expanded, omitting the attribute when the command has no expandable content.
    /// </summary>
    protected string ExpandedString => Expanded switch
    {
        true => "true",
        false => "false",
        _ => null,
    };

    /// <summary>
    /// Specifies the application command label and accessible name, such as File.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Prevents activating the application command. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Reflects whether the associated backstage or menu is expanded for assistive technology.
    /// </summary>
    /// <remarks>
    /// Leave unset for a command without expandable content.
    /// </remarks>
    [Parameter] public bool? Expanded { get; set; }

    /// <summary>
    /// Occurs when the application button is activated. Use this callback to open backstage or perform an application command.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Defines custom application button content, replacing the default label.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}