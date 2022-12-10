#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseColumnableComponent : BaseComponent, IColumnableComponent, IDisposable
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentRowableContext?.NotifyColumnableInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentRowableContext?.NotifyColumnableRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ColumnSize != null && !PreventColumnSize )
            builder.Append( ColumnSize.Class( ClassProvider, ParentRowableContext, this ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the column size generator should be skipped. Used to override the use of column sizes by some of the providers.
    /// </summary>
    protected virtual bool PreventColumnSize => false;

    /// <summary>
    /// Cascaded component that is a container for this component.
    /// </summary>
    [CascadingParameter] protected IRowableContext ParentRowableContext { get; set; }

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
    /// Specifies the content to be rendered inside this <see cref="BaseColumnableComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}