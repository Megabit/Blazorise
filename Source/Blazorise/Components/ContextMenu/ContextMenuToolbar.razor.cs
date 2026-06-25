#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a compact horizontal command group inside a <see cref="ContextMenuBody"/>.
/// </summary>
public partial class ContextMenuToolbar : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuToolbar() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuToolbar"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}