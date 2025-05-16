#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Component that represents the caption of a table element, providing the table an <see href="https://developer.mozilla.org/en-US/docs/Glossary/Accessible_description">accessible description</see>.
/// </summary>
public partial class TableCaption : BaseComponent
{
    #region Members

    private TableCaptionSide side;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableCaption() );
        builder.Append( ClassProvider.TableCaptionSide( Side ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the position of the table caption relative to the table.
    /// </summary>
    [Parameter]
    public TableCaptionSide Side
    {
        get => side;
        set
        {
            if ( side == value )
                return;

            side = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}