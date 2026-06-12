#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a color editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerColorProperty
{
    private Task Clear()
    {
        return Changed.InvokeAsync( null );
    }

    private Task OnChanged( string value )
    {
        return Changed.InvokeAsync( string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current color value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Raised when the color value changes.
    /// </summary>
    [Parameter] public EventCallback<string> Changed { get; set; }
}