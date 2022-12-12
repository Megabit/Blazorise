#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseColumnComponent : BaseComponent, IColumnComponent, IDisposable
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentRowState?.NotifyColumnInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentRowState?.NotifyColumnRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ColumnSize != null && !PreventColumnSize )
            builder.Append( ColumnSize.Class( ClassProvider, ParentRowState, this ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the column size generator should be skipped. Used to override the use of column sizes by some of the providers.
    /// </summary>
    protected virtual bool PreventColumnSize => false;

    /// <summary>
    /// Cascaded row state from a container of this component.
    /// </summary>
    [CascadingParameter] protected IRowState ParentRowState { get; set; }

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