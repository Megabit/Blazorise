#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays a ribbon command using a provider-native button.
/// </summary>
public partial class RibbonButton : BaseRibbonItem
{
    #region Members

    private Button buttonRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default command padding.
    /// </summary>
    public RibbonButton()
    {
        Padding = Blazorise.Padding.Is1;
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
    /// Handles activation of the command button.
    /// </summary>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
        => Disabled ? Task.CompletedTask : HandleClick( eventArgs );

    /// <summary>
    /// Invokes the command through the shared interaction path.
    /// </summary>
    protected virtual Task HandleClick( MouseEventArgs eventArgs ) => Clicked.InvokeAsync( eventArgs );

    /// <summary>
    /// Moves keyboard focus to the command.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the command into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => buttonRef.Focus( scrollToElement );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the command's pressed state.
    /// </summary>
    protected virtual bool IsPressed => false;

    /// <summary>
    /// Occurs when the command is activated.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    #endregion
}