#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Element which specify a term.
/// </summary>
public partial class DescriptionListTerm : BaseTypographyComponent, IColumnComponent
{
    #region Members

    private IFluentColumn columnSize;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DescriptionListTerm() );

        if ( ColumnSize is not null )
            builder.Append( ColumnSize.Class( false, ClassProvider ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

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