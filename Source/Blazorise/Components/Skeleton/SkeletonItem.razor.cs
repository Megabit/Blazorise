#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents an item within a skeleton component, used for displaying placeholder content with optional column sizing.
/// </summary>
public partial class SkeletonItem : BaseComponent
{
    #region Members

    /// <summary>
    /// Represents the column size configuration for the skeleton item.
    /// </summary>
    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <summary>
    /// Builds the CSS classes for the skeleton item component.
    /// </summary>
    /// <param name="builder">The <see cref="ClassBuilder"/> used to construct the CSS classes.</param>
    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.SkeletonItem() );

        if ( ColumnSize is not null && !PreventColumnSize )
            builder.Append( ColumnSize.Class( false, ClassProvider ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the column size generator should be skipped. 
    /// This property can be overridden by derived classes to control whether the column size should be applied.
    /// </summary>
    protected virtual bool PreventColumnSize => false;

    /// <summary>
    /// Gets or sets the column size configuration for the skeleton item.
    /// </summary>
    /// <value>
    /// An <see cref="IFluentColumn"/> that specifies the column size for the skeleton item.
    /// </value>
    [Parameter]
    public IFluentColumn ColumnSize
    {
        get => columnSize;
        set
        {
            columnSize = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the child content to be rendered inside the skeleton item component.
    /// </summary>
    /// <value>
    /// A <see cref="RenderFragment"/> that represents the content to be displayed within the skeleton item.
    /// </value>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}