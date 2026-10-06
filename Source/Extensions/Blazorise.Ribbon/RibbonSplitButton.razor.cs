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
    #region Methods

    /// <summary>
    /// Handles activation of the default action.
    /// </summary>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
        => Disabled ? Task.CompletedTask : HandleClick( eventArgs );

    /// <summary>
    /// Invokes the default action.
    /// </summary>
    protected virtual Task HandleClick( MouseEventArgs eventArgs ) => Clicked.InvokeAsync( eventArgs );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets the menu label after falling back to the command text.
    /// </summary>
    protected string EffectiveMenuLabel => MenuLabel ?? Text;

    /// <summary>
    /// Gets or sets the accessible name of the menu toggle.
    /// </summary>
    [Parameter] public string MenuLabel { get; set; }

    /// <summary>
    /// Gets or sets the direction in which the menu opens.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Gets or sets content replacing the default action button content.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Occurs when the default action is activated.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    #endregion
}