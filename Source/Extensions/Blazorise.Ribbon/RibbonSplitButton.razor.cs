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
    /// Uses <see cref="MenuLabel"/> as the menu toggle accessible name, falling back to the command text.
    /// </summary>
    protected string EffectiveMenuLabel => MenuLabel ?? Text;

    /// <summary>
    /// Specifies the accessible name of the menu toggle. When omitted, uses <see cref="BaseRibbonItem.Text"/>.
    /// </summary>
    [Parameter] public string MenuLabel { get; set; }

    /// <summary>
    /// Controls the direction in which the menu opens. Defaults to <see cref="Direction.Down"/>.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Defines custom content for the default action button, replacing its default icon and label.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }

    /// <summary>
    /// Occurs when the default action button is activated. Activating the menu toggle opens the menu instead.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    #endregion
}