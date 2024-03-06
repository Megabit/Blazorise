#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Card titles are used by adding subtitle to a heading tag. Subtitles are generally placed under the <see cref="CardTitle"/>.
/// </summary>
public partial class CardSubtitle : BaseTypographyComponent
{
    #region Members

    private int size = 6;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CardSubtitle( InsideHeader ) );
        builder.Append( ClassProvider.CardSubtitleSize( InsideHeader, Size ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the subtitle is placed inside if card header.
    /// </summary>
    protected bool InsideHeader => ParentCardHeader is not null;

    /// <summary>
    /// Number from 1 to 6 that defines the subtitle size where the smaller number means larger text.
    /// </summary>
    /// <remarks>
    /// todo: change to enum
    /// </remarks>
    [Parameter]
    public int Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="CardHeader"/> component.
    /// </summary>
    [CascadingParameter] public CardHeader ParentCardHeader { get; set; }

    #endregion
}