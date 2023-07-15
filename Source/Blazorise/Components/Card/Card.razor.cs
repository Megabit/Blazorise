#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A card is a flexible and extensible content container. It includes options for headers and footers,
/// a wide variety of content, contextual background colors, and powerful display options.
/// </summary>
public partial class Card : BaseComponent
{
    #region Members

    private bool isWhiteText;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Card() );
        builder.Append( ClassProvider.CardWhiteText(), WhiteText );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if card is placed inside of a deck.
    /// </summary>
    protected bool InsideDeck => ParentCardDeck != null;

    /// <summary>
    /// Sets the white text when using the darker background.
    /// </summary>
    [Parameter]
    public bool WhiteText
    {
        get => isWhiteText;
        set
        {
            isWhiteText = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Card"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent deck component.
    /// </summary>
    [CascadingParameter] protected CardDeck ParentCardDeck { get; set; }

    #endregion
}