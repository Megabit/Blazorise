#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a set of rows summarizing the columns of the table.
/// </summary>
public partial class TableFooter : BaseDraggableComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableFooter() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder
            .OpenElement( "tfoot" )
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

        builder.CloseElement(); // </tfoot>

        base.BuildRenderTree( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableFooter"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}