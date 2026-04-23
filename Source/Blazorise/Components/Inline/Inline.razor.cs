#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for horizontally stacked input components.
/// </summary>
public partial class Inline : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Inline() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the inline container.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
