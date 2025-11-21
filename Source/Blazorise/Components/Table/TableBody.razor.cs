#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Table Body element encapsulates a set of table rows, indicating that they comprise the body of the table.
/// </summary>
public partial class TableBody : BaseDraggableComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableBody() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder
            .OpenElement( "tbody" )
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

        builder.CloseElement(); // </tbody>

        base.BuildRenderTree( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}