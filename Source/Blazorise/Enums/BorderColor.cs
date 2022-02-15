namespace Blazorise
{
    /// <summary>
    /// Predefined set of contextual border colors.
    /// </summary>
    public record struct BorderColor
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A default target contructor.
        /// </summary>
        /// <param name="name">Named value of the enum.</param>
        public BorderColor( string name )
        {
            Name = name;
        }

        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        public static readonly BorderColor None = new( (string)null );

        /// <summary>
        /// Primary color.
        /// </summary>
        public static readonly BorderColor Primary = new( "primary" );

        /// <summary>
        /// Secondary color.
        /// </summary>
        public static readonly BorderColor Secondary = new( "secondary" );

        /// <summary>
        /// Success color.
        /// </summary>
        public static readonly BorderColor Success = new( "success" );

        /// <summary>
        /// Danger color.
        /// </summary>
        public static readonly BorderColor Danger = new( "danger" );

        /// <summary>
        /// Warning color.
        /// </summary>
        public static readonly BorderColor Warning = new( "warning" );

        /// <summary>
        /// Info color.
        /// </summary>
        public static readonly BorderColor Info = new( "info" );

        /// <summary>
        /// Light color.
        /// </summary>
        public static readonly BorderColor Light = new( "light" );

        /// <summary>
        /// Dark color.
        /// </summary>
        public static readonly BorderColor Dark = new( "dark" );

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly BorderColor White = new( "white" );
    }
}
