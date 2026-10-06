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
    /// Indicates whether the containing ribbon displays commands in a single compact row.
    /// </summary>
    protected bool IsSimplified => ParentRibbonState?.DisplayMode == RibbonDisplayMode.Simplified;

    /// <summary>
    /// Resolves the command arrangement, stacking explicit rows in classic mode and arranging commands horizontally in simplified mode.
    /// </summary>
    protected IFluentFlex EffectiveItemsFlex => Layout == RibbonGroupLayout.Rows && !IsSimplified ? Blazorise.Flex.Column.JustifyContent.Center : Blazorise.Flex.Row.NoWrap.AlignItems.Stretch;

    /// <summary>
    /// Identifies the caption used to label the command group for assistive technology.
    /// </summary>
    protected string CaptionElementId => $"{ElementId}-caption";

    /// <summary>
    /// Uses <see cref="LauncherLabel"/> as the launcher accessible name, falling back to the group caption.
    /// </summary>
    protected string EffectiveLauncherLabel => LauncherLabel ?? Text;

    /// <summary>
    /// Specifies the group caption displayed below its commands in classic mode and used as the group's accessible name.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Controls how commands, columns, and rows are arranged in classic mode.
    /// </summary>
    /// <remarks>
    /// Simplified mode always arranges commands horizontally. Defaults to <see cref="RibbonGroupLayout.Columns"/>.
    /// </remarks>
    [Parameter] public RibbonGroupLayout Layout { get; set; }

    /// <summary>
    /// Displays a launcher beside the group caption in classic mode, allowing access to additional options.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool ShowLauncher { get; set; }

    /// <summary>
    /// Prevents activating the group launcher. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool LauncherDisabled { get; set; }

    /// <summary>
    /// Specifies the accessible name of the group launcher. When omitted, uses <see cref="Text"/>.
    /// </summary>
    [Parameter] public string LauncherLabel { get; set; }

    /// <summary>
    /// Occurs when the group launcher is activated. Use this callback to display a dialog or additional options.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> LauncherClicked { get; set; }

    /// <summary>
    /// Defines the grouped commands. Use <see cref="RibbonColumn"/> to stack commands or <see cref="RibbonRow"/> for rows of commands and inputs.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shares the containing ribbon's selection, display mode, and rendering policy with this component.
    /// </summary>
    [CascadingParameter] protected RibbonState ParentRibbonState { get; set; }

    #endregion
}