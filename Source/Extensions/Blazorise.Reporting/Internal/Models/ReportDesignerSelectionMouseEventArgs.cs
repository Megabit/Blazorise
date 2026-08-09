#region Using directives
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Event arguments for mouse selection actions raised by report designer items.
/// </summary>
/// <param name="key">Key of the report designer item associated with the mouse action.</param>
/// <param name="mouseEventArgs">Original Blazor mouse event arguments.</param>
public sealed class ReportDesignerSelectionMouseEventArgs( string key, MouseEventArgs mouseEventArgs )
{
    /// <summary>
    /// Key of the report designer item associated with the mouse action.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Original Blazor mouse event arguments.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; } = mouseEventArgs;
}