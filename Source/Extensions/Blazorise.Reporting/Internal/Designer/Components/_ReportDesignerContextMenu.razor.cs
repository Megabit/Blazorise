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
/// Renders the floating designer context menu shell.
/// </summary>
public partial class _ReportDesignerContextMenu
{
    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( parameters.TryGetValue<double>( nameof( X ), out var paramX ) && paramX != X )
             || ( parameters.TryGetValue<double>( nameof( Y ), out var paramY ) && paramY != Y ) )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"left:{X}px" );
        builder.Append( $"top:{Y}px" );
    }

    /// <summary>
    /// Indicates that the context menu is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Horizontal client coordinate of the context menu.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Vertical client coordinate of the context menu.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Context menu commands.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}