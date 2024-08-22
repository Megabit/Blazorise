#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The bottom layout with the default style, in which any element can be nested, and must be placed in <see cref="Layout"/>.
/// </summary>
public partial class LayoutFooter : BaseComponent
{
    #region Members

    private bool @fixed;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.LayoutFooter() );
        builder.Append( ClassProvider.LayoutFooterFixed(), Fixed );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// If true footer will be fixed to the bottom of the page.
    /// </summary>
    [Parameter]
    public bool Fixed
    {
        get => @fixed;
        set
        {
            @fixed = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="LayoutFooter"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}