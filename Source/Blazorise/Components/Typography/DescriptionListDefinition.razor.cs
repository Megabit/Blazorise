#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Element which specify a term.
/// </summary>
public partial class DescriptionListDefinition : BaseTypographyComponent, IColumnableComponent, IDisposable
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
        builder.Append( ClassProvider.DescriptionListDefinition() );

        if ( ColumnSize != null )
            builder.Append( ColumnSize.Class( ClassProvider, ParentRowable, this ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Cascaded component that is a container for this component.
    /// </summary>
    [CascadingParameter] protected IRowableComponent ParentRowable { get; set; }

    /// <summary>
    /// Determines how much space will be used by the definition inside of the description list row.
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

    #endregion
}