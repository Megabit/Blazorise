#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main <see cref="Jumbotron"/> heading text.
/// </summary>
public partial class JumbotronTitle : BaseComponent
{
    #region Members

    private JumbotronTitleSize size = JumbotronTitleSize.Is1;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.JumbotronTitle( Size ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Gets the name of the heading element based on current <see cref="Size"/> parameter.
    /// </summary>
    /// <returns>The h element name.</returns>
    protected string GetHeadingElementName() => size switch
    {
        JumbotronTitleSize.Is1 => "h1",
        JumbotronTitleSize.Is2 => "h2",
        JumbotronTitleSize.Is3 => "h3",
        JumbotronTitleSize.Is4 => "h4",
        _ => "h1",
    };

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the jumbotron text size.
    /// </summary>
    [Parameter]
    public JumbotronTitleSize Size
    {
        get => size;
        set
        {
            size = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="JumbotronTitle"/> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}