#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper that represents a column in a flexbox grid.
/// </summary>
public partial class Column : BaseColumnComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        // Only add column classname if there are no custom sizes defined!
        // If any provider need to have base classname then it needs to add
        // it in ClassProvider.Column(...) builder.
        builder.Append( ClassProvider.Column( InsideGrid, ColumnSize?.HasSizes == true ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ColumnGutter( Gutter ) );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Column grid spacing, we recommend setting it to (16 + 8n). (n stands for natural number.)
    /// </summary>
    [CascadingParameter] public (int Horizontal, int Vertical) Gutter { get; set; }

    #endregion
}