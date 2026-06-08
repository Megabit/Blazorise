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
/// Renders the marquee selection rectangle on the report designer surface.
/// </summary>
public partial class _ReportDesignerSelectionBox
{
    private const string Key = "selection-box";

    private string Style => StyleNames;

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
        builder.Append( $"left:{X + LeftOffset}px" );
        builder.Append( $"top:{Y}px" );
        builder.Append( $"width:{Width}px" );
        builder.Append( $"height:{Height}px" );
    }

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
}