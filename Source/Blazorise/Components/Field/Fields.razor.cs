﻿#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for multiple <see cref="Field"/> component that needs to be placed in a flexbox grid row.
/// </summary>
public partial class Fields : BaseColumnComponent
{
    #region Members

    private string label;

    private string help;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Fields() );

        if ( ColumnSize is not null )
        {
            builder.Append( ClassProvider.FieldsColumn() );
        }

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties   

    /// <summary>
    /// Sets the field label.
    /// </summary>
    [Parameter]
    public string Label
    {
        get => label;
        set
        {
            label = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the field help-text positioned bellow the field.
    /// </summary>
    [Parameter]
    public string Help
    {
        get => help;
        set
        {
            help = value;

            DirtyClasses();
        }
    }

    #endregion
}