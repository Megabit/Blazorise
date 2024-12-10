#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Indicate the current page's location within a navigational hierarchy.
/// </summary>
public partial class Breadcrumb : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Breadcrumb() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the breadcrumb activation mode.
    /// </summary>
    [Parameter] public BreadcrumbMode Mode { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Breadcrumb"/> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}