#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a combined theme color and custom color editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerThemeColorProperty
{
    #region Methods

    private Task Clear()
    {
        return Task.WhenAll(
            ThemeChanged.InvokeAsync( null ),
            CustomChanged.InvokeAsync( null ) );
    }

    private Task OnThemeChanged( string value )
    {
        return ThemeChanged.InvokeAsync( string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    private Task OnCustomChanged( string value )
    {
        return CustomChanged.InvokeAsync( string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    #endregion

    #region Properties

    private IReadOnlyList<(string Value, string Text)> ResolvedThemeOptions => ThemeOptions ?? System.Array.Empty<(string Value, string Text)>();

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current semantic theme color name.
    /// </summary>
    [Parameter] public string ThemeValue { get; set; }

    /// <summary>
    /// Current custom color value.
    /// </summary>
    [Parameter] public string CustomValue { get; set; }

    /// <summary>
    /// Available semantic theme color names.
    /// </summary>
    [Parameter] public IReadOnlyList<(string Value, string Text)> ThemeOptions { get; set; }

    /// <summary>
    /// Raised when the semantic theme color changes.
    /// </summary>
    [Parameter] public EventCallback<string> ThemeChanged { get; set; }

    /// <summary>
    /// Raised when the custom color changes.
    /// </summary>
    [Parameter] public EventCallback<string> CustomChanged { get; set; }

    #endregion
}