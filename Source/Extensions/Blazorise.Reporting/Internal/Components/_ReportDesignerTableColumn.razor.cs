#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a cached report designer table column cell.
/// </summary>
public partial class _ReportDesignerTableColumn
{
    #region Methods

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

    #endregion

    #region Properties

    private string Style => StyleNames;

    /// <summary>
    /// Table column definition rendered by the table cell.
    /// </summary>
    [Parameter] public ReportTableColumnDefinition Column { get; set; }

    #endregion
}