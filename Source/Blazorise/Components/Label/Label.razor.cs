#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A label for a form fields.
/// </summary>
public partial class Label : BaseComponent
{
    #region Members

    private LabelType type = LabelType.None;

    private Cursor cursor;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.LabelType( Type ) );
        builder.Append( ClassProvider.LabelCursor( Cursor ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name of the input element to which the label is connected.
    /// </summary>
    [Parameter] public string For { get; set; }

    /// <summary>
    /// Label type that can better indicate the connected input element.
    /// </summary>
    [Parameter]
    public LabelType Type
    {
        get => type;
        set
        {
            type = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the mouse cursor when mouse od placed over the label.
    /// </summary>
    [Parameter]
    public Cursor Cursor
    {
        get => cursor;
        set
        {
            cursor = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the content rendered inside the label.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
