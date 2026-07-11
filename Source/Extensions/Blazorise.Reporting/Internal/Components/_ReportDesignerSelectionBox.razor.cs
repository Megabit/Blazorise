#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the marquee selection rectangle on the report designer surface.
/// </summary>
public partial class _ReportDesignerSelectionBox
{
    #region Members

    private const string Key = "selection-box";

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( parameters.TryGetValue<double>( nameof( X ), out var paramX ) && paramX != X )
             || ( parameters.TryGetValue<double>( nameof( Y ), out var paramY ) && paramY != Y )
             || ( parameters.TryGetValue<double>( nameof( Width ), out var paramWidth ) && paramWidth != Width )
             || ( parameters.TryGetValue<double>( nameof( Height ), out var paramHeight ) && paramHeight != Height )
             || ( parameters.TryGetValue<double>( nameof( LeftOffset ), out var paramLeftOffset ) && paramLeftOffset != LeftOffset ) )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{ReportMeasurementConverter.FormatCssPixelValue( ReportMeasurementConverter.ToCssPixelValue( X ) + LeftOffset )}" );
        builder.Append( $"top:{ReportMeasurementConverter.ToCssPixelString( Y )}" );
        builder.Append( $"width:{ReportMeasurementConverter.ToCssPixelString( Width )}" );
        builder.Append( $"height:{ReportMeasurementConverter.ToCssPixelString( Height )}" );
    }

    #endregion

    #region Properties

    private string Style => StyleNames;

    /// <summary>
    /// Left selection coordinate.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Top selection coordinate.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Selection width.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Selection height.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// Horizontal offset applied when the band rail is visible.
    /// </summary>
    [Parameter] public double LeftOffset { get; set; }

    #endregion
}