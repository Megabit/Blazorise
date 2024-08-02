#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Group a series of buttons together on a single line.
/// </summary>
public partial class Buttons : BaseComponent
{
    #region Members

    private ButtonsRole role = ButtonsRole.Addons;

    private Orientation orientation = Orientation.Horizontal;

    private Size size = Size.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Buttons( Role, Orientation ) );
        builder.Append( ClassProvider.ButtonsSize( Size ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the role of the button group.
    /// </summary>
    [Parameter]
    public ButtonsRole Role
    {
        get => role;
        set
        {
            role = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the button group orientation mode.
    /// </summary>
    [Parameter]
    public Orientation Orientation
    {
        get => orientation;
        set
        {
            orientation = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Change the size of multiple buttons at once.
    /// </summary>
    [Parameter]
    public Size Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Buttons"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}