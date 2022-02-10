namespace Blazorise
{
    /// <summary>
    /// Predefined set of contextual background colors.
    /// </summary>
    public record struct Background
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A default target contructor.
        /// </summary>
        /// <param name="name">Named value of the enum.</param>
        public Background( string name )
        {
            Name = name;
        }

        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        public static readonly Background None = new( (string)null );

        /// <summary>
        /// Primary color.
        /// </summary>
        public static readonly Background Primary = new( "primary" );

        /// <summary>
        /// Secondary color.
        /// </summary>
        public static readonly Background Secondary = new( "secondary" );

        /// <summary>
        /// Success color.
        /// </summary>
        public static readonly Background Success = new( "success" );

        /// <summary>
        /// Danger color.
        /// </summary>
        public static readonly Background Danger = new( "danger" );

        /// <summary>
        /// Warning color.
        /// </summary>
        public static readonly Background Warning = new( "warning" );

        /// <summary>
        /// Info color.
        /// </summary>
        public static readonly Background Info = new( "info" );

        /// <summary>
        /// Light color.
        /// </summary>
        public static readonly Background Light = new( "light" );

        /// <summary>
        /// Dark color.
        /// </summary>
        public static readonly Background Dark = new( "dark" );

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly Background White = new( "white" );

        /// <summary>
        /// Transparent color.
        /// </summary>
        public static readonly Background Transparent = new( "transparent" );

        /// <summary>
        /// Body color. Note that body color must be defined through the <see cref="Theme"/> options and
        /// not every provider supports it!
        /// </summary>
        public static readonly Background Body = new( "body" );
    }
}
