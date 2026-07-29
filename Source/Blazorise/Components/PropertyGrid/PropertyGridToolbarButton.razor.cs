#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a command inside a <see cref="PropertyGridToolbar"/>.
/// </summary>
public partial class PropertyGridToolbarButton : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.PropertyGridToolbarButton() );

        base.BuildClasses( builder );
    }

    private Task OnClicked( MouseEventArgs eventArgs ) => Clicked.InvokeAsync();

    #endregion

    #region Properties

    /// <summary>
    /// Defines the button icon.
    /// </summary>
    [Parameter] public object Icon { get; set; }

    /// <summary>
    /// Defines the button text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Defines the accessible button title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Defines whether the button text is rendered.
    /// </summary>
    [Parameter] public bool ShowText { get; set; }

    /// <summary>
    /// Defines whether the button is active.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Defines whether the button is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Defines the button color.
    /// </summary>
    [Parameter] public Color Color { get; set; } = Color.Secondary;

    /// <summary>
    /// Defines whether the button uses an outlined style.
    /// </summary>
    [Parameter] public bool Outline { get; set; } = true;

    /// <summary>
    /// Defines the button size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Defines custom button content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}