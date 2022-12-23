#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A description list is a list of items with a description or definition of each item.
/// </summary>
public partial class DescriptionList : BaseTypographyComponent
{
    #region Members

    private bool row;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DescriptionList() );

        if ( Row )
            builder.Append( ClassProvider.Row() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies that description list will be arranged in a rows and columns.
    /// </summary>
    [Parameter]
    public bool Row
    {
        get => row;
        set
        {
            row = value;

            DirtyClasses();
        }
    }

    #endregion
}