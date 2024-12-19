#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Static text that can be placed inside of an <see cref="Addon"/>.
/// </summary>
public partial class AddonLabel : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.AddonLabel() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="AddonLabel"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}