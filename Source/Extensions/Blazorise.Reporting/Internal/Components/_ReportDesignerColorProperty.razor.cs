#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a color editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerColorProperty
{
    #region Members

    private const string MixedValue = "__b_report_mixed__";

    private static readonly (string Value, string Text)[] DefaultNamedOptions =
    [
        ( string.Empty, "Default" ),
        ( "Black", "Black" ),
        ( "White", "White" ),
        ( "Red", "Red" ),
        ( "Green", "Green" ),
        ( "Blue", "Blue" ),
        ( "Yellow", "Yellow" ),
        ( "Cyan", "Cyan" ),
        ( "Magenta", "Magenta" ),
        ( "Gray", "Gray" ),
        ( "LightGray", "Light gray" ),
        ( "DarkGray", "Dark gray" ),
        ( "Navy", "Navy" ),
        ( "Maroon", "Maroon" ),
        ( "Olive", "Olive" ),
        ( "Purple", "Purple" ),
        ( "Teal", "Teal" ),
        ( "Silver", "Silver" ),
        ( "Orange", "Orange" ),
        ( "Transparent", "Transparent" ),
    ];

    #endregion

    #region Methods

    private Task Clear()
    {
        return Changed.InvokeAsync( ReportColor.Default );
    }

    private Task OnNameChanged( string value )
    {
        if ( value == MixedValue )
            return Task.CompletedTask;

        ReportColor color = ReportColor.FromString( value );

        return !Mixed && color.Equals( Value )
            ? Task.CompletedTask
            : Changed.InvokeAsync( color );
    }

    private Task OnCustomChanged( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return Task.CompletedTask;

        ReportColor color = ReportColor.FromString( value );

        return !Mixed && string.Equals( color.ToCssString(), Value.ToCssString(), StringComparison.OrdinalIgnoreCase )
            ? Task.CompletedTask
            : Changed.InvokeAsync( color );
    }

    #endregion

    #region Properties

    private string SelectedName => Mixed
        ? MixedValue
        : Value.Kind == ReportColorKind.Named || Value.Kind == ReportColorKind.Transparent
        ? Value.Name
        : string.Empty;

    private string CustomValue => Mixed
        ? null
        : Value.Kind == ReportColorKind.Rgb
        ? FormattableString.Invariant( $"#{Value.Red:X2}{Value.Green:X2}{Value.Blue:X2}" )
        : Value.Kind == ReportColorKind.Named
            ? Value.ToCssString()
            : null;

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current color value.
    /// </summary>
    [Parameter] public ReportColor Value { get; set; }

    /// <summary>
    /// Indicates that selected elements have different values.
    /// </summary>
    [Parameter] public bool Mixed { get; set; }

    /// <summary>
    /// Raised when the color value changes.
    /// </summary>
    [Parameter] public EventCallback<ReportColor> Changed { get; set; }

    #endregion
}