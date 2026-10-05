#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays a ribbon command with a bindable checked state.
/// </summary>
public class RibbonToggleButton : RibbonButton
{
    #region Methods

    /// <inheritdoc/>
    protected override async Task HandleClick( MouseEventArgs eventArgs )
    {
        Checked = !Checked;

        await CheckedChanged.InvokeAsync( Checked );
        await base.HandleClick( eventArgs );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool IsPressed => Checked;

    /// <summary>
    /// Gets or sets whether the command is checked.
    /// </summary>
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    #endregion
}