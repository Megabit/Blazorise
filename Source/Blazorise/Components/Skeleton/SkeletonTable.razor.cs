#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A skeleton table that can be used to represent a skeleton of a table.
/// </summary>
public partial class SkeletonTable : BaseComponent
{
    #region Properties

    /// <summary>
    /// Defined the animation style applied to the skeleton.
    /// </summary>
    [Parameter] public SkeletonAnimation Animation { get; set; }

    /// <summary>
    /// Specifies the number of rows to be rendered. Default is 3.
    /// </summary>
    [Parameter] public int Rows { get; set; } = 3;

    /// <summary>
    /// Specifies the number of columns to be rendered. Default is 5.
    /// </summary>
    [Parameter] public int Columns { get; set; } = 5;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Skeleton"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
