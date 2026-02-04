#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Card titles are used by adding title to a heading tag.
/// </summary>
public partial class CardTitle : BaseTypographyComponent
{
    #region Members

    private HeadingSize? size;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CardTitle( InsideHeader ) );
        builder.Append( ClassProvider.CardTitleSize( InsideHeader, Size ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the title is placed inside a card header.
    /// </summary>
    protected bool InsideHeader => ParentCardHeader is not null;

    /// <summary>
    /// Gets the title size number.
    /// </summary>
    protected string SizeNumber => Size switch
    {
        HeadingSize.Is1 => "1",
        HeadingSize.Is2 => "2",
        HeadingSize.Is3 => "3",
        HeadingSize.Is4 => "4",
        HeadingSize.Is5 => "5",
        HeadingSize.Is6 => "6",
        _ => "6",
    };

    /// <summary>
    /// Defines the title size where the smaller number means larger text.
    /// </summary>
    [Parameter]
    public HeadingSize? Size
    {
        get => size;
        set
        {
            if ( size == value )
                return;

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