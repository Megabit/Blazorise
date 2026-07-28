#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a named or custom CSS color property editor.
/// </summary>
public partial class PropertyGridColorItem : BasePropertyGridEditorItem
{
    #region Members

    private const string MixedValue = "__b_property_grid_mixed__";

    private static readonly PropertyGridSelectOption<string>[] defaultNamedColors =
    [
        new( string.Empty, "Default" ),
        new( "Black", "Black" ),
        new( "White", "White" ),
        new( "Red", "Red" ),
        new( "Green", "Green" ),
        new( "Blue", "Blue" ),
        new( "Yellow", "Yellow" ),
        new( "Cyan", "Cyan" ),
        new( "Magenta", "Magenta" ),
        new( "Gray", "Gray" ),
        new( "LightGray", "Light gray" ),
        new( "DarkGray", "Dark gray" ),
        new( "Navy", "Navy" ),
        new( "Maroon", "Maroon" ),
        new( "Olive", "Olive" ),
        new( "Purple", "Purple" ),
        new( "Teal", "Teal" ),
        new( "Silver", "Silver" ),
        new( "Orange", "Orange" ),
        new( "Transparent", "Transparent" ),
    ];

    #endregion

    #region Methods

    private Task Clear()
        => ValueChanged.InvokeAsync( string.Empty );

    private Task OnNameChanged( string value )
    {
        if ( value == MixedValue || string.Equals( value, Value, StringComparison.OrdinalIgnoreCase ) && !Mixed )
            return Task.CompletedTask;

        return ValueChanged.InvokeAsync( value );
    }

    private Task OnCustomChanged( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) || string.Equals( value, Value, StringComparison.OrdinalIgnoreCase ) && !Mixed )
            return Task.CompletedTask;

        return ValueChanged.InvokeAsync( value );
    }

    private string FindNamedColorValue()
    {
        foreach ( PropertyGridSelectOption<string> option in ResolvedNamedColors )
        {
            if ( string.Equals( option.Value, Value, StringComparison.OrdinalIgnoreCase ) )
                return option.Value;
        }

        return string.Empty;
    }

    #endregion

    #region Properties

    private IReadOnlyList<PropertyGridSelectOption<string>> ResolvedNamedColors => NamedColors ?? defaultNamedColors;

    private string SelectedName => Mixed
        ? MixedValue
        : FindNamedColorValue();

    private string CustomValue => Mixed || string.IsNullOrWhiteSpace( Value ) ? null : Value;

    /// <summary>
    /// Gets or sets the CSS color value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the CSS color value changes.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Defines the named colors available in the select editor.
    /// </summary>
    [Parameter] public IReadOnlyList<PropertyGridSelectOption<string>> NamedColors { get; set; }

    /// <summary>
    /// Defines whether the color can be cleared.
    /// </summary>
    [Parameter] public bool Clearable { get; set; } = true;

    /// <summary>
    /// Defines the accessible title for the clear action.
    /// </summary>
    [Parameter] public string ClearTitle { get; set; } = "Clear color";

    #endregion
}