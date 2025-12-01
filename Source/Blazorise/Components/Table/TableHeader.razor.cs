#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a set of rows defining the head of the columns of the table.
/// </summary>
public partial class TableHeader : BaseDraggableComponent
{
    #region Members

    private ThemeContrast themeContrast;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableHeader() );
        builder.Append( ClassProvider.TableHeaderThemeContrast( ThemeContrast ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder
            .OpenElement( "thead" )
            .Id( ElementId )
            .Class( ClassNames )
            .Style( StyleNames )
            .Draggable( DraggableString );

        // build drag-and-drop related events
        BuildDraggableEventsRenderTree( builder );

        if ( Attributes is not null )
            builder.Attributes( Attributes );

        builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

        if ( ChildContent is not null )
            builder.Content( ChildContent );

        builder.CloseElement(); // </thead>

        base.BuildRenderTree( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sets the preferred color contrast for the header.
    /// </summary>
    [Parameter]
    public ThemeContrast ThemeContrast
    {
        get => themeContrast;
        set
        {
            themeContrast = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}