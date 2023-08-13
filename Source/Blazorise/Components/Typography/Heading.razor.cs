#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Heading component is used for titles or subtitles that you want to display on a webpage.
/// </summary>
public partial class Heading : BaseTypographyComponent
{
    #region Members

    private HeadingSize headingSize = HeadingSize.Is3;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.HeadingSize( headingSize ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the heading tag name.
    /// </summary>
    protected string TagName => string.IsNullOrEmpty( AlternativeTagName )
        ? $"h{SizeNumber}"
        : AlternativeTagName;

    /// <summary>
    /// Gets the heading size number.
    /// </summary>
    protected string SizeNumber => Size switch
    {
        HeadingSize.Is1 => "1",
        HeadingSize.Is2 => "2",
        HeadingSize.Is3 => "3",
        HeadingSize.Is4 => "4",
        HeadingSize.Is5 => "5",
        HeadingSize.Is6 => "6",
        _ => "3",
    };

    /// <summary>
    /// Gets or sets the heading size.
    /// </summary>
    [Parameter]
    public HeadingSize Size
    {
        get => headingSize;
        set
        {
            if ( headingSize == value )
                return;

            headingSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the alternative tag name for the heading element. Default is set to <c>h1</c>.
    /// </summary>
    [Parameter] public string AlternativeTagName { get; set; }

    #endregion
}