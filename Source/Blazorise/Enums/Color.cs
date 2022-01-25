using System;

namespace Blazorise
{
    /// <summary>
    /// Predefined set of contextual colors.
    /// </summary>
    public record struct Color
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A default target contructor.
        /// </summary>
        /// <param name="name">Named value of the enum.</param>
        public Color( string name )
        {
            Name = name;
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
    }
}
