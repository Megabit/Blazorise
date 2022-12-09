#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

public interface IColumnableComponent
{
    IFluentColumn ColumnSize { get; }
}

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseColumnableComponent : BaseComponent, IColumnableComponent
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentRowable?.NotifyColumnInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentRowable?.NotifyColumnDestroyed( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ColumnSize != null && !PreventColumnSize )
            builder.Append( ColumnSize.Class( ClassProvider, ParentRowable, this ) );

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
    [CascadingParameter] protected IRowableComponent ParentRowable { get; set; }

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