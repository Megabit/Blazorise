#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A generic close button for dismissing content like modals and alerts.
/// </summary>
public partial class CloseButton : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CloseButton() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the item onclick event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task ClickHandler( MouseEventArgs eventArgs )
    {
        // We must have priority over what get's closed once we click on close button.
        // For example, there can be Alert placed inside of Modal, and Close Button inside of Alert.
        // And we don't want to close both Alert and Modal in that case.
        if ( IsAutoClose )
        {
            if ( ParentAlert is not null )
            {
                await ParentAlert.Hide();
            }
            else if ( ParentModal is not null )
            {
                await ParentModal.Hide();
            }
        }

        await Clicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns true if parent component should be automatically closed.
    /// </summary>
    protected bool IsAutoClose
        => AutoClose.GetValueOrDefault( Options?.AutoCloseParent ?? true );

    /// <summary>
    /// Gets the string representing the disabled state.
    /// </summary>
    protected string DisabledString
        => Disabled ? bool.TrueString.ToLowerInvariant() : null;

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Flag to indicate that the button is not responsive for user interaction.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// If true, the parent <see cref="Alert"/> or <see cref="Modal"/> with be automatically closed
    /// when <see cref="CloseButton"/> button is placed inside of them.
    /// </summary>
    [Parameter] public bool? AutoClose { get; set; }

    /// <summary>
    /// Cascaded <see cref="Alert"/> component in which this <see cref="CloseButton"/> is placed.
    /// </summary>
    [CascadingParameter] protected Alert ParentAlert { get; set; }

    /// <summary>
    /// Cascaded <see cref="Modal"/> component in which this <see cref="CloseButton"/> is placed.
    /// </summary>
    [CascadingParameter] protected Modal ParentModal { get; set; }

    /// <summary>
    /// Cascaded <see cref="Toast"/> component in which this <see cref="CloseButton"/> is placed.
    /// </summary>
    [CascadingParameter] protected Toast ParentToast { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="CloseButton"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}