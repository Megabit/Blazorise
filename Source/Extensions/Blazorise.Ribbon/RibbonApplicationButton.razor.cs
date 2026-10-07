#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    private ComponentParameterInfo<bool> paramExpanded;

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
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Expanded, out paramExpanded );

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = buttonRef.ElementRef;
        }

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
    protected virtual async Task HandleClick( MouseEventArgs eventArgs )
    {
        if ( HasBackstage )
        {
            await ParentWorkspace.SetBackstageVisible( true );
        }
        else if ( CanToggleExpanded )
        {
            Expanded = !Expanded;

            await ExpandedChanged.InvokeAsync( Expanded );
        }

        await Clicked.InvokeAsync( eventArgs );
    }

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

    private bool HasBackstage => ParentWorkspaceState?.BackstageElementId is not null;

    private bool CanToggleExpanded => ParentWorkspace is null && ( paramExpanded.Defined || ExpandedChanged.HasDelegate );

    /// <summary>
    /// Resolves expansion from workspace state or the standalone parameter.
    /// </summary>
    protected bool? EffectiveExpanded
        => HasBackstage ? ParentWorkspaceState.BackstageVisible : CanToggleExpanded ? Expanded : null;

    /// <summary>
    /// Serializes the expanded state for aria-expanded, omitting the attribute when the command has no expandable content.
    /// </summary>
    protected string ExpandedString => EffectiveExpanded switch
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
    /// Controls whether independently associated backstage or menu content is expanded.
    /// </summary>
    /// <remarks>
    /// Bind this parameter to standalone backstage visibility. Inside a workspace, expansion is owned by the workspace.
    /// Leave unset for a command without expandable content. Defaults to <c>false</c>.
    /// </remarks>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>
    /// Occurs when a standalone application button toggles its expanded state.
    /// </summary>
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// Occurs when the application button is activated, after any backstage or expansion change.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Defines custom application button content, replacing the default label.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets the optional workspace that coordinates backstage interaction.
    /// </summary>
    [CascadingParameter] protected RibbonWorkspace ParentWorkspace { get; set; }

    /// <summary>
    /// Gets the workspace's backstage association and visibility.
    /// </summary>
    [CascadingParameter] protected RibbonWorkspaceState ParentWorkspaceState { get; set; }

    #endregion
}