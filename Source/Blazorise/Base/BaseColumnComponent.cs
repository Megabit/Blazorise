#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseColumnComponent : BaseComponent, IColumnComponent
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ColumnSize is not null && !PreventColumnSize )
            builder.Append( ColumnSize.Class( InsideGrid, ClassProvider ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the column size generator should be skipped. Used to override the use of column sizes by some of the providers.
    /// </summary>
    protected virtual bool PreventColumnSize => false;

    /// <summary>
    /// Indicates if the column is placed inside of <see cref="Grid"/> component as a direct descendant.
    /// </summary>
    protected bool InsideGrid => ParentGrid is not null;

    /// <summary>
    /// Cascaded <see cref="Row"/> component in which this <see cref="Column"/> is placed.
    /// </summary>
    [CascadingParameter] public Row ParentRow { get; set; }

    /// <summary>
    /// Cascaded <see cref="Grid"/> component in which this <see cref="Column"/> is placed.
    /// </summary>
    [CascadingParameter] public Grid ParentGrid { get; set; }

    /// <summary>
    /// Defines the column sizes.
    /// </summary>
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
    /// Specifies the content to be rendered inside this <see cref="BaseColumnComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}