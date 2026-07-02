#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a cached report designer table column cell.
/// </summary>
public partial class _ReportDesignerTableColumn
{
    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportTableColumnDefinition>( nameof( Column ), out _ ) )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( Column.Width )}" );
    }

    /// <summary>
    /// Table column definition rendered by the table cell.
    /// </summary>
    [Parameter] public ReportTableColumnDefinition Column { get; set; }
}