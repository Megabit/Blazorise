#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Arranges related ribbon commands above a group caption.
/// </summary>
public partial class RibbonGroup : BaseComponent
{
    #region Constructors

    /// <summary>
    /// Initializes the default group spacing and separator.
    /// </summary>
    public RibbonGroup()
    {
        Border = Blazorise.Border.Is1.OnEnd;
        Padding = Blazorise.Padding.Is2.OnX;
        Flex = Blazorise.Flex.Column.Shrink.Is0;
        Gap = Blazorise.Gap.Is1;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles activation of the group launcher.
    /// </summary>
    protected Task OnLauncherHandler( MouseEventArgs eventArgs ) => HandleLauncher( eventArgs );

    /// <summary>
    /// Invokes the application-owned group dialog.
    /// </summary>
    protected virtual Task HandleLauncher( MouseEventArgs eventArgs )
        => LauncherDisabled ? Task.CompletedTask : LauncherClicked.InvokeAsync( eventArgs );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets whether the ribbon uses a single command row.
    /// </summary>
    protected bool IsSimplified => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified;

    /// <summary>
    /// Gets the command arrangement after resolving the display mode.
    /// </summary>
    protected IFluentFlex EffectiveItemsFlex => Layout == RibbonGroupLayout.Rows && !IsSimplified ? Blazorise.Flex.Column.JustifyContent.Center : Blazorise.Flex.Row.NoWrap.AlignItems.Stretch;

    /// <summary>
    /// Gets the group caption element identifier.
    /// </summary>
    protected string CaptionElementId => $"{ElementId}-caption";

    /// <summary>
    /// Gets the launcher label after falling back to the group text.
    /// </summary>
    protected string EffectiveLauncherLabel => LauncherLabel ?? Text;

    /// <summary>
    /// Gets or sets the group caption.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets the command arrangement.
    /// </summary>
    [Parameter] public RibbonGroupLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets whether the group dialog launcher is shown.
    /// </summary>
    [Parameter] public bool ShowLauncher { get; set; }

    /// <summary>
    /// Gets or sets whether the launcher is disabled.
    /// </summary>
    [Parameter] public bool LauncherDisabled { get; set; }

    /// <summary>
    /// Gets or sets the accessible name of the launcher.
    /// </summary>
    [Parameter] public string LauncherLabel { get; set; }

    /// <summary>
    /// Occurs when the group dialog launcher is activated.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> LauncherClicked { get; set; }

    /// <summary>
    /// Gets or sets commands, explicit ribbon columns, or ribbon rows.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the containing ribbon state.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}