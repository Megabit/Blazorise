#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

/// <summary>
/// Displays a non-interactive label within sidebar navigation.
/// </summary>
public partial class SidebarLabel : BaseComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-label" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SidebarLabel"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}