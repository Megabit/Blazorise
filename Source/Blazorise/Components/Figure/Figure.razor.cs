#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for piece of content like images, than can display optional caption.
/// </summary>
public partial class Figure : BaseComponent
{
    #region Members

    private FigureSize size = FigureSize.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Figure() );
        builder.Append( ClassProvider.FigureSize( Size ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the figure size.
    /// </summary>
    [Parameter]
    public FigureSize Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the content rendered inside the figure.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
