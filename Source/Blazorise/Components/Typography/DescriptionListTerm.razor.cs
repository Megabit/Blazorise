#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Element which specify a term.
/// </summary>
public partial class DescriptionListTerm : BaseTypographyComponent, IColumnableComponent, IDisposable
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        RowableContext?.NotifyColumnableInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            RowableContext?.NotifyColumnableRemoved( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DescriptionListTerm() );

        if ( ColumnSize != null )
            builder.Append( ColumnSize.Class( ClassProvider, RowableContext, this ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    [CascadingParameter] protected IRowableContext RowableContext { get; } = new RowableContext();

    /// <summary>
    /// Determines how much space will be used by the term inside of the description list row.
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