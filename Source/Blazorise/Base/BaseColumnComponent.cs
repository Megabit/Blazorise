#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseColumnComponent<TClasses, TStyles> : BaseComponent<TClasses, TStyles>, IColumnComponent
    where TClasses : ComponentClasses
    where TStyles : ComponentStyles
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
    /// The cascaded <see cref="Row"/> component that contains this <see cref="Column"/>.
    /// </summary>
    [CascadingParameter] public Row ParentRow { get; set; }

    /// <summary>
    /// The cascaded <see cref="Grid"/> component that contains this <see cref="Column"/>.
    /// </summary>
    [CascadingParameter] public Grid ParentGrid { get; set; }

    /// <summary>
    /// Defines the sizing configuration for the column, supporting responsive layouts and custom size definitions.
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
    /// Specifies the content to render inside this <see cref="BaseColumnComponent"/>.
    /// </summary>
    /// <remarks>
    /// Use this property to define custom child elements or components to be displayed within the column.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseColumnComponent : BaseColumnComponent<ComponentClasses, ComponentStyles>
{
}