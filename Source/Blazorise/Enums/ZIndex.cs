#region Using directives
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a provider-defined stacking layer or an explicit CSS z-index.
/// </summary>
public sealed record ZIndex
{
    private ZIndex( string name )
    {
        Name = name;
    }

    private ZIndex( int value )
    {
        Value = value;
    }

    /// <summary>
    /// Creates an explicit z-index, including zero and negative values.
    /// </summary>
    /// <param name="value">The CSS z-index.</param>
    public static implicit operator ZIndex( int value ) => new( value );

    /// <summary>
    /// Creates an explicit z-index, or uses the default when the value is null.
    /// </summary>
    /// <param name="value">The optional CSS z-index.</param>
    public static implicit operator ZIndex( int? value ) => value.HasValue ? new( value.Value ) : Default;

    /// <inheritdoc/>
    public override string ToString() => Name ?? Value?.ToString( CultureInfo.InvariantCulture ) ?? string.Empty;

    /// <summary>
    /// Gets the named provider layer, or null for default and numeric values.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the explicit numeric z-index, or null for default and named values.
    /// </summary>
    public int? Value { get; }

    /// <summary>
    /// Gets whether existing component styling determines the stacking level.
    /// </summary>
    public bool IsDefault => Name is null && Value is null;

    /// <summary>
    /// Leaves the component's existing stacking behavior unchanged.
    /// </summary>
    public static readonly ZIndex Default = new( (string)null );

    /// <summary>
    /// Uses the provider's zero stacking level, distinct from leaving z-index unspecified.
    /// </summary>
    public static readonly ZIndexLevel Is0 = new( new ZIndex( "0" ), new ZIndex( "0" ) );

    /// <summary>
    /// Uses the provider's first positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndexLevel Is1 = new( new ZIndex( "1" ), new ZIndex( "n1" ) );

    /// <summary>
    /// Uses the provider's second positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndexLevel Is2 = new( new ZIndex( "2" ), new ZIndex( "n2" ) );

    /// <summary>
    /// Uses the provider's third positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndexLevel Is3 = new( new ZIndex( "3" ), new ZIndex( "n3" ) );

    /// <summary>
    /// Uses the provider's dropdown layer.
    /// </summary>
    public static readonly ZIndex Dropdown = new( "dropdown" );

    /// <summary>
    /// Uses the provider's sticky layer without changing positioning.
    /// </summary>
    public static readonly ZIndex Sticky = new( "sticky" );

    /// <summary>
    /// Uses the provider's fixed layer without changing positioning.
    /// </summary>
    public static readonly ZIndex Fixed = new( "fixed" );

    /// <summary>
    /// Uses the provider's sidebar backdrop layer.
    /// </summary>
    public static readonly ZIndex SidebarBackdrop = new( "sidebar-backdrop" );

    /// <summary>
    /// Uses the provider's sidebar layer for application navigation.
    /// </summary>
    public static readonly ZIndex Sidebar = new( "sidebar" );

    /// <summary>
    /// Uses the provider's offcanvas backdrop layer.
    /// </summary>
    public static readonly ZIndex OffcanvasBackdrop = new( "offcanvas-backdrop" );

    /// <summary>
    /// Uses the provider's offcanvas layer.
    /// </summary>
    public static readonly ZIndex Offcanvas = new( "offcanvas" );

    /// <summary>
    /// Uses the provider's modal backdrop layer.
    /// </summary>
    public static readonly ZIndex ModalBackdrop = new( "modal-backdrop" );

    /// <summary>
    /// Uses the provider's modal layer.
    /// </summary>
    public static readonly ZIndex Modal = new( "modal" );

    /// <summary>
    /// Uses the provider's popover layer.
    /// </summary>
    public static readonly ZIndex Popover = new( "popover" );

    /// <summary>
    /// Uses the provider's tooltip layer.
    /// </summary>
    public static readonly ZIndex Tooltip = new( "tooltip" );

    /// <summary>
    /// Uses the provider's toast layer.
    /// </summary>
    public static readonly ZIndex Toast = new( "toast" );

    /// <summary>
    /// Uses the provider's snackbar layer.
    /// </summary>
    public static readonly ZIndex Snackbar = new( "snackbar" );

    /// <summary>
    /// Uses the provider's on-screen keyboard layer.
    /// </summary>
    public static readonly ZIndex OnScreenKeyboard = new( "on-screen-keyboard" );
}

/// <summary>
/// A numbered stacking level that supports a negative counterpart.
/// Implicitly converts to <see cref="ZIndex"/>.
/// </summary>
public readonly struct ZIndexLevel
{
    internal ZIndexLevel( ZIndex value, ZIndex negative )
    {
        Value = value;
        Negative = new( negative );
    }

    internal ZIndex Value { get; }

    /// <summary>
    /// Uses the negative counterpart of this stacking level. The zero level remains zero.
    /// </summary>
    public ZIndexVariant Negative { get; }

    /// <summary>
    /// Implicitly converts the numbered level to its corresponding <see cref="ZIndex"/> value.
    /// </summary>
    /// <param name="zIndexLevel">The numbered level to convert.</param>
    public static implicit operator ZIndex( ZIndexLevel zIndexLevel ) => zIndexLevel.Value;

    /// <inheritdoc/>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}

/// <summary>
/// A wrapper returned by a stacking level modifier to prevent further chaining.
/// Implicitly converts back to <see cref="ZIndex"/>.
/// </summary>
public readonly struct ZIndexVariant
{
    internal ZIndexVariant( ZIndex zIndex ) => Value = zIndex;

    internal ZIndex Value { get; }

    /// <summary>
    /// Implicitly converts the variant to its corresponding <see cref="ZIndex"/> value.
    /// </summary>
    /// <param name="zIndexVariant">The variant to convert.</param>
    public static implicit operator ZIndex( ZIndexVariant zIndexVariant ) => zIndexVariant.Value;

    /// <inheritdoc/>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}