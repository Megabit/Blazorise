#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

public partial class SkeletonTable : BaseComponent
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    [Parameter] public int Rows { get; set; } = 3;

    [Parameter] public int Columns { get; set; } = 5;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Skeleton"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
