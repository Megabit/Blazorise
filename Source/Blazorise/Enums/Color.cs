#region Using directives
using System;
#endregion

namespace Blazorise
{

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
        /// No color will be applied to an element.
        /// </summary>
        public static readonly Color None = new( (string)null );

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
        /// Link color.
        /// </summary>
        public static readonly Color Link = new( "link" );

        public Color Accent1 => new Color( this, "accent-1" );

        public Color Accent2 => new Color( this, "accent-2" );

        public Color Accent3 => new Color( this, "accent-3" );
    }
}
