#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A larger, slightly more opinionated heading style.
/// </summary>
public partial class DisplayHeading : BaseTypographyComponent
{
    #region Members

    private DisplayHeadingSize displayHeadingSize = DisplayHeadingSize.Is2;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DisplayHeadingSize( Size ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the heading element based on current <see cref="Size"/> parameter.
    /// </summary>
    protected string TagName => string.IsNullOrEmpty( AlternativeTagName )
        ? "h1"
        : AlternativeTagName;

    /// <summary>
    /// Gets or sets the display heading size.
    /// </summary>
    [Parameter]
    public DisplayHeadingSize Size
    {
        get => displayHeadingSize;
        set
        {
            if ( displayHeadingSize == value )
                return;

            displayHeadingSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the alternative tag name for the heading element. Default is set to <c>h1</c>.
    /// </summary>
    [Parameter] public string AlternativeTagName { get; set; }

    #endregion
}