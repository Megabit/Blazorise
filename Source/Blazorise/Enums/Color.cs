#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Predefined set of contextual colors.
/// </summary>
public record Color : Enumeration<Color>
{
    /// <inheritdoc/>
    public Color( string name ) : base( name )
    {
    }

    /// <inheritdoc/>
    private Color( Color parent, string name ) : base( parent, name )
    {
    }

    /// <summary>
    /// Creates the new custom color based on the supplied enum value.
    /// </summary>
    /// <param name="name">Name value of the enum.</param>
    public static implicit operator Color( string name )
    {
        return new Color( name );
    }

    /// <summary>
    /// No color will be applied to an element, meaning it will appear as default to whatever current theme is set to.
    /// </summary>
    public static readonly Color Default = new( null );

    /// <summary>
    /// Primary color.
    /// </summary>
    public static readonly Color Primary = new( "primary" );

    /// <summary>
    /// Secondary color.
    /// </summary>
    public static readonly Color Secondary = new( "secondary" );

    /// <summary>
    /// Success color.
    /// </summary>
    public static readonly Color Success = new( "success" );

    /// <summary>
    /// Danger color.
    /// </summary>
    public static readonly Color Danger = new( "danger" );

    /// <summary>
    /// Warning color.
    /// </summary>
    public static readonly Color Warning = new( "warning" );

    /// <summary>
    /// Info color.
    /// </summary>
    public static readonly Color Info = new( "info" );

    /// <summary>
    /// Light color.
    /// </summary>
    public static readonly Color Light = new( "light" );

    /// <summary>
    /// Dark color.
    /// </summary>
    public static readonly Color Dark = new( "dark" );

    /// <summary>
    /// Link color. Can only be used on alerts and buttons.
    /// </summary>
    public static readonly Color Link = new( "link" );
}