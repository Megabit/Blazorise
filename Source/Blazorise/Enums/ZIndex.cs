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
    /// Uses the provider's negative stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndex IsNegative1 = new( "n1" );

    /// <summary>
    /// Uses the provider's zero stacking level, distinct from leaving z-index unspecified.
    /// </summary>
    public static readonly ZIndex Is0 = new( "0" );

    /// <summary>
    /// Uses the provider's first positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndex Is1 = new( "1" );

    /// <summary>
    /// Uses the provider's second positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndex Is2 = new( "2" );

    /// <summary>
    /// Uses the provider's third positive stacking level. The CSS value is provider-defined.
    /// </summary>
    public static readonly ZIndex Is3 = new( "3" );

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