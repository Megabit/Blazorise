#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Opens a menu of preset colors, with access to a full color picker for custom colors.
/// </summary>
public partial class RibbonColorPicker : BaseRibbonItem
{
    #region Members

    private static readonly string[] simpleColors =
    [
        "#000000", "#FFFFFF", "#FF0000", "#FFFF00",
        "#00B050", "#0070C0", "#7030A0", "#7F7F7F",
    ];

    private static readonly string[] extendedColors =
    [
        "#FFFFFF", "#000000", "#E7E6E6", "#44546A", "#4472C4", "#ED7D31", "#A5A5A5", "#FFC000", "#5B9BD5", "#70AD47",
        "#F2F2F2", "#7F7F7F", "#DBDBDB", "#D6DCE4", "#D9E2F3", "#FBE4D5", "#EDEDED", "#FFF2CC", "#DEEBF7", "#E2EFD9",
        "#D9D9D9", "#595959", "#C5C5C5", "#ADB9CA", "#B4C6E7", "#F8CBAD", "#DBDBDB", "#FFE599", "#BDD7EE", "#C5E0B3",
        "#BFBFBF", "#3F3F3F", "#AEAAAA", "#8497B0", "#8EAADB", "#F4B183", "#C9C9C9", "#FFD966", "#9DC3E6", "#A8D08D",
        "#A5A5A5", "#262626", "#757171", "#323F4F", "#2F5496", "#C55A11", "#7B7B7B", "#BF9000", "#2E75B5", "#538135",
        "#7F7F7F", "#0C0C0C", "#3B3838", "#222A35", "#203864", "#833C0B", "#525252", "#7F6000", "#1F4E78", "#375623",
    ];

    private static readonly string[] standardColors =
    [
        "#C00000", "#FF0000", "#FFC000", "#FFFF00", "#92D050", "#00B050", "#00B0F0", "#0070C0", "#002060", "#7030A0",
    ];

    private Dropdown dropdownRef;

    private DropdownToggle toggleRef;

    private ColorPicker pickerRef;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the default color command presentation.
    /// </summary>
    public RibbonColorPicker()
    {
        MoreColorsClassBuilder = new( BuildMoreColorsClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            ElementRef = dropdownRef.ElementRef;
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    /// <summary>
    /// Builds the native menu item appearance for the full picker trigger.
    /// </summary>
    protected virtual void BuildMoreColorsClasses( ClassBuilder builder ) => builder.Append( ClassProvider.DropdownItem() );

    /// <summary>
    /// Handles changes received from the color picker.
    /// </summary>
    protected Task OnValueChangedHandler( string value ) => HandleValueChanged( value );

    /// <summary>
    /// Handles visibility changes received from the preset menu.
    /// </summary>
    protected Task OnVisibleChangedHandler( bool visible ) => HandleVisibleChanged( visible );

    /// <summary>
    /// Updates menu visibility before notifying the application.
    /// </summary>
    protected virtual async Task HandleVisibleChanged( bool visible )
    {
        if ( Visible == visible )
        {
            return;
        }

        Visible = visible;

        if ( visible && ShowMoreColors && pickerRef is not null )
        {
            await pickerRef.Hide();
        }

        await VisibleChanged.InvokeAsync( visible );
    }

    /// <summary>
    /// Handles the application-defined automatic color command.
    /// </summary>
    protected Task OnAutomaticClickedHandler() => HandleAutomaticClicked();

    /// <summary>
    /// Notifies the application to apply its default color and restores command focus.
    /// </summary>
    protected virtual async Task HandleAutomaticClicked()
    {
        if ( IsDisabled )
        {
            return;
        }

        await AutomaticClicked.InvokeAsync();
        await Focus( false );
    }

    /// <summary>
    /// Handles the No Color command.
    /// </summary>
    protected Task OnNoColorClickedHandler() => HandleNoColorClicked();

    /// <summary>
    /// Clears the color and restores focus after the native menu item closes the menu.
    /// </summary>
    protected virtual async Task HandleNoColorClicked()
    {
        if ( IsDisabled )
        {
            return;
        }

        await HandleValueChanged( null );
        await Focus( false );
    }

    /// <summary>
    /// Handles keyboard dismissal while focus is inside the preset menu.
    /// </summary>
    protected Task OnMenuKeyDownHandler( KeyboardEventArgs eventArgs ) => HandleMenuKeyDown( eventArgs );

    /// <summary>
    /// Closes the preset menu and returns keyboard focus to the ribbon command on Escape.
    /// </summary>
    protected virtual async Task HandleMenuKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( !Visible || eventArgs.Key is not ( "Escape" or "Esc" ) )
        {
            return;
        }

        await dropdownRef.Hide();
        await Focus( false );
    }

    /// <summary>
    /// Handles activation of the full color picker from More Colors.
    /// </summary>
    protected Task OnMoreColorsClickedHandler() => HandleMoreColorsClicked();

    /// <summary>
    /// Anchors the full picker to the ribbon command before closing the preset menu.
    /// </summary>
    protected virtual async Task HandleMoreColorsClicked()
    {
        if ( IsDisabled )
        {
            return;
        }

        await pickerRef.JSModule.Show( pickerRef.ElementRef, pickerRef.ElementId, ToggleElementId );
        await dropdownRef.Hide();
    }

    /// <summary>
    /// Updates the color and optionally closes the preset menu before restoring command focus.
    /// </summary>
    protected virtual async Task HandleColorSelected( string value )
    {
        if ( IsDisabled )
        {
            return;
        }

        await HandleValueChanged( value );

        if ( HideAfterPaletteSelect )
        {
            await dropdownRef.Hide();
            await Focus( false );
        }
    }

    /// <summary>
    /// Updates the selected color before notifying the application.
    /// </summary>
    protected virtual async Task HandleValueChanged( string value )
    {
        if ( IsDisabled || Value == value )
        {
            return;
        }

        Value = value;

        await ValueChanged.InvokeAsync( value );
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Compares preset colors across supported literal formats, including RGB and hexadecimal.
    /// </summary>
    protected bool IsColorSelected( string color )
    {
        if ( string.IsNullOrEmpty( color ) || string.IsNullOrEmpty( Value ) )
        {
            return false;
        }

        if ( string.Equals( color, Value, StringComparison.OrdinalIgnoreCase ) )
        {
            return true;
        }

        return HtmlColorCodeParser.TryParse( color, out var parsedColor )
            && HtmlColorCodeParser.TryParse( Value, out var parsedSelectedColor )
            && parsedColor.ToArgb() == parsedSelectedColor.ToArgb();
    }

    /// <summary>
    /// Moves keyboard focus to the color menu toggle.
    /// </summary>
    /// <param name="scrollToElement">
    /// Whether to scroll the toggle into view.
    /// </param>
    public Task Focus( bool scrollToElement = true ) => toggleRef.Focus( scrollToElement );

    /// <summary>
    /// Opens the preset color menu when the command is enabled and editable.
    /// </summary>
    public Task Show() => IsDisabled ? Task.CompletedTask : dropdownRef.Show();

    /// <summary>
    /// Closes the preset menu and the full color picker.
    /// </summary>
    public async Task Hide()
    {
        await dropdownRef.Hide();

        if ( ShowMoreColors && pickerRef is not null )
        {
            await pickerRef.Hide();
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Determines whether color selection is unavailable.
    /// </summary>
    protected bool IsDisabled => Disabled || ReadOnly;

    /// <summary>
    /// Determines whether either preset grid contains colors and should be displayed.
    /// </summary>
    protected bool HasPalette => ShowPalette && ( EffectivePalette.Length > 0 || EffectiveStandardPalette.Length > 0 );

    /// <summary>
    /// Resolves explicit primary colors before falling back to the selected preset.
    /// </summary>
    protected string[] EffectivePalette => Palette ?? ( PalettePreset switch
    {
        RibbonColorPalette.Simple => simpleColors,
        RibbonColorPalette.Standard => standardColors,
        _ => extendedColors,
    } );

    /// <summary>
    /// Resolves the optional second grid, using standard colors for the Extended preset.
    /// </summary>
    protected string[] EffectiveStandardPalette => StandardPalette ?? ( Palette is null && PalettePreset == RibbonColorPalette.Extended ? standardColors : Array.Empty<string>() );

    /// <summary>
    /// Resolves the primary heading from an explicit label or the selected palette.
    /// </summary>
    protected string EffectivePaletteText => PaletteText ?? ( Palette is not null ? "Colors" : PalettePreset switch
    {
        RibbonColorPalette.Simple => "Colors",
        RibbonColorPalette.Standard => "Standard Colors",
        _ => "Theme Colors",
    } );

    /// <summary>
    /// Resolves the column count within the supported range and available color count.
    /// </summary>
    protected int EffectivePaletteColumns => Math.Clamp( Math.Min( PaletteColumns, Math.Max( EffectivePalette.Length, EffectiveStandardPalette.Length ) ), 1, 12 );

    /// <summary>
    /// Resolves the grid layout shared by the preset sections.
    /// </summary>
    protected IFluentGridColumns EffectiveGridColumns => new FluentGridColumns().WithGridColumnsSize( (GridColumnsSize)EffectivePaletteColumns );

    /// <summary>
    /// Identifies the command used to anchor the full color picker.
    /// </summary>
    protected string ToggleElementId => $"{ElementId}-toggle";

    /// <summary>
    /// Gets the selected-color indicator background, clearing it when no color is selected.
    /// </summary>
    protected Background EffectiveIndicatorBackground => string.IsNullOrEmpty( Value ) ? Blazorise.Background.Transparent : new( Value );

    /// <summary>
    /// Builds the provider-native menu item classes.
    /// </summary>
    protected ClassBuilder MoreColorsClassBuilder { get; }

    /// <summary>
    /// Provides the rendered classes for More Colors.
    /// </summary>
    protected string MoreColorsClassNames => MoreColorsClassBuilder.Class;

    /// <summary>
    /// Specifies the selected color as a string supported by <see cref="ColorPicker"/>.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the selected color changes. Supplies the new color value for two-way binding.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Displays the selected color value beside the command preview when the picker is closed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c> to keep ribbon commands compact.
    /// </remarks>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>
    /// Prevents changing the selected color. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Selects the predefined colors used when no explicit palette is supplied.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="RibbonColorPalette.Extended"/>, which includes tint and shade rows and a standard-color section.
    /// </remarks>
    [Parameter] public RibbonColorPalette PalettePreset { get; set; } = RibbonColorPalette.Extended;

    /// <summary>
    /// Supplies explicit primary colors in row order, replacing the predefined palette.
    /// </summary>
    /// <remarks>
    /// A null value uses <see cref="PalettePreset"/>. An empty array hides the primary grid.
    /// Supplying explicit colors also removes the preset's second grid unless <see cref="StandardPalette"/> is supplied.
    /// </remarks>
    [Parameter] public string[] Palette { get; set; }

    /// <summary>
    /// Displays the preset swatch grids in the color menu. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowPalette { get; set; } = true;

    /// <summary>
    /// Automatically closes the color menu after selecting a preset. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool HideAfterPaletteSelect { get; set; } = true;

    /// <summary>
    /// Provides custom localization for color picker text, overriding the default translations.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    /// <summary>
    /// Controls whether the preset color menu is open. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool Visible { get; set; }

    /// <summary>
    /// Occurs when the preset menu opens or closes, supporting two-way binding.
    /// </summary>
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Controls the direction in which the preset menu opens. Defaults to <see cref="Direction.Down"/>.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Provides a custom heading and accessible name for the primary swatch grid.
    /// </summary>
    /// <remarks>
    /// A null value uses the preset's heading, or Colors for an explicit palette.
    /// An empty string hides the heading.
    /// </remarks>
    [Parameter] public string PaletteText { get; set; }

    /// <summary>
    /// Controls the maximum number of swatches per row in both grids. Defaults to ten.
    /// </summary>
    /// <remarks>
    /// The supported range is one through twelve. Palettes with fewer colors use fewer columns.
    /// </remarks>
    [Parameter] public int PaletteColumns { get; set; } = 10;

    /// <summary>
    /// Supplies explicit colors for a second swatch grid beneath the primary palette.
    /// </summary>
    /// <remarks>
    /// A null value uses the standard-color section of the Extended preset when no explicit primary palette is supplied.
    /// An empty array hides this section.
    /// </remarks>
    [Parameter] public string[] StandardPalette { get; set; }

    /// <summary>
    /// Provides the heading and accessible name of the second swatch grid. Defaults to Standard Colors.
    /// </summary>
    [Parameter] public string StandardPaletteText { get; set; } = "Standard Colors";

    /// <summary>
    /// Displays the Automatic command above the palettes. Defaults to <c>false</c>.
    /// </summary>
    [Parameter] public bool ShowAutomatic { get; set; }

    /// <summary>
    /// Provides the Automatic command label.
    /// </summary>
    [Parameter] public string AutomaticText { get; set; } = "Automatic";

    /// <summary>
    /// Occurs when Automatic is selected, allowing the application to apply its default color.
    /// </summary>
    /// <remarks>
    /// This event does not assign a color value. The application owns the meaning of Automatic.
    /// </remarks>
    [Parameter] public EventCallback AutomaticClicked { get; set; }

    /// <summary>
    /// Displays the No Color command in the palette menu. Defaults to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Selecting No Color clears <see cref="Value"/> to null and notifies <see cref="ValueChanged"/>.
    /// </remarks>
    [Parameter] public bool ShowNoColor { get; set; } = true;

    /// <summary>
    /// Provides the command label used to clear the selected color.
    /// </summary>
    [Parameter] public string NoColorText { get; set; } = "No Color";

    /// <summary>
    /// Displays the command that opens the full picker for a custom color. Defaults to <c>true</c>.
    /// </summary>
    [Parameter] public bool ShowMoreColors { get; set; } = true;

    /// <summary>
    /// Provides the command label used to open the full picker.
    /// </summary>
    [Parameter] public string MoreColorsText { get; set; } = "More Colors…";

    #endregion
}