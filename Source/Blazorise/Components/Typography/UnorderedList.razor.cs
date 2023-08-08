#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// An unordered list created using the &lt;ul&gt; element.
/// </summary>
public partial class UnorderedList : BaseTypographyComponent
{
    #region Members

    private bool unstyled;

    private string listStyleImage;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.UnorderedList() );
        builder.Append( ClassProvider.UnorderedListUnstyled( Unstyled ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( !string.IsNullOrEmpty( ListStyleImage ) )
        {
            builder.Append( $"list-style-image: url('{ListStyleImage}')" );
        }

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Remove the default <c>list-style</c> and left margin on list items (immediate children only).
    /// </summary>
    [Parameter]
    public bool Unstyled
    {
        get => unstyled;
        set
        {
            if ( unstyled == value )
                return;

            unstyled = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the marker images for list items. The paremeter accepts Base64 encoded string that represents an image, or a URL.
    /// </summary>
    [Parameter]
    public string ListStyleImage
    {
        get => listStyleImage;
        set
        {
            if ( listStyleImage == value )
                return;

            listStyleImage = value;

            DirtyClasses();
        }
    }

    #endregion
}