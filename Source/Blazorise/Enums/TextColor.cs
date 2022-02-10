namespace Blazorise
{
    /// <summary>
    /// Predefined set of contextual text colors.
    /// </summary>
    public record struct TextColor
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A default target contructor.
        /// </summary>
        /// <param name="name">Named value of the enum.</param>
        public TextColor( string name )
        {
            Name = name;
        }

        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        public static readonly TextColor None = new( (string)null );

        /// <summary>
        /// Primary color.
        /// </summary>
        public static readonly TextColor Primary = new( "primary" );

        /// <summary>
        /// Secondary color.
        /// </summary>
        public static readonly TextColor Secondary = new( "secondary" );

        /// <summary>
        /// Success color.
        /// </summary>
        public static readonly TextColor Success = new( "success" );

        /// <summary>
        /// Danger color.
        /// </summary>
        public static readonly TextColor Danger = new( "danger" );

        /// <summary>
        /// Warning color.
        /// </summary>
        public static readonly TextColor Warning = new( "warning" );

        /// <summary>
        /// Info color.
        /// </summary>
        public static readonly TextColor Info = new( "info" );

        /// <summary>
        /// Light color.
        /// </summary>
        public static readonly TextColor Light = new( "light" );

        /// <summary>
        /// Dark color.
        /// </summary>
        public static readonly TextColor Dark = new( "dark" );

        /// <summary>
        /// Body color.
        /// </summary>
        public static readonly TextColor Body = new( "body" );

        /// <summary>
        /// Muted color.
        /// </summary>
        public static readonly TextColor Muted = new( "muted" );

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly TextColor White = new( "white" );

        /// <summary>
        /// Black text with 50% opacity on white background.
        /// </summary>
        public static readonly TextColor Black50 = new( "black-50" );

        /// <summary>
        /// White text with 50% opacity on black background.
        /// </summary>
        public static readonly TextColor White50 = new( "white-50" );
    }
}
